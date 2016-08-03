using Foundation;
using System;
using UIKit;
using CoreGraphics;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.APIRoot;
using ProcessDashboard.DBWrapper;
using Exception = System.Exception;
using Task = ProcessDashboard.DTO.Task;

namespace ProcessDashboard.iOS
{
    public partial class TaskTimeLogViewController : UIViewController
    {

		//UILabel ProjectNameLabel, TaskNameLabel, TimelogsLabel;
		List<TimeLogEntry> timeLogCache;
		//UITableView TaskTimeLogTable;

		// This ID is used to fetch the time logs. It is set by the previous view controller
		public string taskId;
		public Task task;

		public TaskTimeLogViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			refreshData();
		}
		private async void refreshData()
		{
			await getTimeLogsOfTask();

			TaskTimeLogTable.Source = new TaskTimeLogTableSource(timeLogCache, this);
			TaskTimeLogTable.ReloadData();
		}

		public async System.Threading.Tasks.Task<int> getTimeLogsOfTask()
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			List<TimeLogEntry> timeLogEntries = await c.GetTimeLogs(AccountStorage.DataSet, 0, null, null,taskId, null);

			timeLogCache = timeLogEntries;

			try
			{
				System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
				System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

				foreach (var proj in timeLogEntries)
				{
					System.Diagnostics.Debug.WriteLine("Task Name : " + proj.Task.FullName);
					System.Diagnostics.Debug.WriteLine("Start Date : " + proj.StartDate);
					System.Diagnostics.Debug.WriteLine("End Date : " + proj.EndDate);
					//  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ProjectNameLabel.Text = task.Project != null ? task.Project.Name : "";
			TaskNameLabel.Text = task.FullName.ToString();

		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier == "eachTimeLogSegue")
			{
				var navctlr = segue.DestinationViewController as TimelogDetailViewController;
				if (navctlr != null)
				{
					var source = TaskTimeLogTable.Source as TaskTimeLogTableSource;
					var rowPath = TaskTimeLogTable.IndexPathForSelectedRow;

					navctlr.SetTaskforTaskTimelog(this, timeLogCache[rowPath.Row]); // to be defined on the TaskDetailViewController
				}
			}
			if (segue.Identifier == "AddTimeLogSegue")
			{

				var controller = segue.DestinationViewController as TimelogDetailViewController;
				TimeLogEntry newTimeLog = new TimeLogEntry();
				newTimeLog.Task = task;
				newTimeLog.StartDate = DateTime.Now;
				controller.CreateTask(this, newTimeLog);
				AddTaskTimelog(newTimeLog);

			}

		}

		public async void DeleteTask(TimeLogEntry log)
		{

			await DeleteATimeLog(log.Id);
			NavigationController.PopViewController(true);
		}

		public async System.Threading.Tasks.Task<int> DeleteATimeLog(int? val)
		{
			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			string timeLogId;
			if (val.HasValue)
			{
				timeLogId = "" + val.Value;

				DeleteRoot tr = await c.DeleteTimeLog("INST-szewf0", timeLogId);
				try
				{
					System.Diagnostics.Debug.WriteLine("** Delete the new Time Log entry **");
					System.Diagnostics.Debug.WriteLine("Status :" + tr.Stat);

				}
				catch (System.Exception e)
				{
					System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
				}
			}
			return 0;
		}

		public async void UpdateTaskTimelog(TimeLogEntry log)
		{
			await UpdateATimeLog(log);
		}

		public async System.Threading.Tasks.Task<int> UpdateATimeLog(TimeLogEntry editedTimeLog)
		{

			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller c = new Controller(service);

			TimeLogEntry tr = await c.UpdateTimeLog("INST-szewf0", editedTimeLog.Id.ToString(), editedTimeLog.Comment, editedTimeLog.StartDate, editedTimeLog.Task.Id, editedTimeLog.LoggedTime, editedTimeLog.InterruptTime, true);
			try
			{
				System.Diagnostics.Debug.WriteLine("** Updated the new Time Log entry **");
				System.Diagnostics.Debug.WriteLine("Updated Logged Time :" + tr.LoggedTime);

			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
			}
			return 0;

		}

		public async void AddTaskTimelog(TimeLogEntry log)
		{
			await TestAddATimeLog(log);
		}

		public async Task<int> TestAddATimeLog(TimeLogEntry log)
		{

			var apiService = new ApiTypes(null);
			var service = new PDashServices(apiService);
			Controller ctrl = new Controller(service);

			int id;
			try
			{
				var tr = await ctrl.AddATimeLog("INST-szewf0", "No Comment", DateTime.UtcNow, log.Task.Id, log.LoggedTime, log.InterruptTime, true);
				Console.WriteLine("** Added a new Time Log entry **");
				Console.WriteLine(tr.Id);
				id = tr.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine("We are in an error state :" + e);
				id = 0;
			}

			// See if adding a new time log entry successfully

			var value = await ctrl.GetTimeLog("INST-szewf0", "" + id);
			Console.WriteLine("Task Name :" + value.Task.FullName);
			Console.WriteLine(value.Id);
			Console.WriteLine("Logged Time :" + value.LoggedTime);
			Console.WriteLine("Interrupt Time :" + value.InterruptTime);
			Console.WriteLine(Util.GetInstance().GetLocalTime(value.StartDate));
			Console.WriteLine(Util.GetInstance().GetLocalTime(value.EndDate));
			Console.WriteLine(value.Comment);

			return id;

		}

    }
}