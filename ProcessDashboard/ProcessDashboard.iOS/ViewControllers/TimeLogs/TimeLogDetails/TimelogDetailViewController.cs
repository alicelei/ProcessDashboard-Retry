using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using SharpMobileCode.ModalPicker;
using ProcessDashboard.Service;
using ProcessDashboard.Service_Access_Layer;
using ProcessDashboard.SyncLogic;
using ProcessDashboard.DTO;


namespace ProcessDashboard.iOS
{
	public partial class TimelogDetailViewController : UIViewController
	{
		TimeLogEntry currentTask { get; set; }
		CustomTimeLogCell currentCell { get; set; }
		public TimeLogPageViewController Delegate { get; set; } // will be used to Save, Delete later
		public TaskTimeLogViewController DelegateforTasktimelog { get; set; } // will be used to Save, Delete later
		public TaskTimeLogViewController DelegateforAddingTimelog { get; set;}
		UIBarButtonItem delete;
		UIBarButtonItem saveButton, cancelButton;
		UIPickerView DeltaPicker, IntPicker;
		UIDatePicker  StartTimePicker;
		string deltaSelectedHour, deltaSelectedMinute;
		string intSelectedHour, intSelectedMinute;
		DateTime startTimeSelectedDate;
		UIToolbar toolbar;

		public TimelogDetailViewController(IntPtr handle) : base(handle)
		{
			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			delete = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (s, e) =>
			{


				UIAlertController actionSheetAlert = UIAlertController.Create(null, "This time log will be deleted.", UIAlertControllerStyle.ActionSheet);

				// Add Actions
				actionSheetAlert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (action) =>
				{
					if (Delegate != null)
					{
						Delegate.DeleteTask(currentTask);
					}
					else if (DelegateforTasktimelog != null)
					{
						DelegateforTasktimelog.DeleteTask(currentTask);
					}
				}));

				actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (action) => Console.WriteLine("Cancel button pressed.")));

				UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = this.View;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				}

				// Display the alertg
				this.PresentViewController(actionSheetAlert, true, null);
				//delete the task time log

			});

			NavigationItem.RightBarButtonItem = delete;

			ProjectNameLabel.Text = currentTask.Task.Project.Name;

			TaskNameLabel.Text = currentTask.Task.FullName;

			StartTimeText.Text = currentTask.StartDate.ToLocalTime().ToString("g");
		
			// set up start time customized UIpicker

			StartTimePicker = new UIDatePicker(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 250, this.View.Frame.Width - 20, 200f));
			StartTimePicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			StartTimePicker.UserInteractionEnabled = true;
			StartTimePicker.Mode = UIDatePickerMode.DateAndTime;
			StartTimePicker.MaximumDate = ConvertDateTimeToNSDate(DateTime.UtcNow.ToLocalTime());

			startTimeSelectedDate = currentTask.StartDate.ToLocalTime();

			StartTimePicker.ValueChanged += (Object sender, EventArgs e) =>
			{
				startTimeSelectedDate  = ConvertNSDateToDateTime((sender as UIDatePicker).Date);
			};

			StartTimePicker.BackgroundColor = UIColor.White;
			StartTimePicker.SetDate(ConvertDateTimeToNSDate(currentTask.StartDate), true);

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar

			saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Bordered, null);

			saveButton.Clicked += (s, e) =>
			{
				this.StartTimeText.Text = startTimeSelectedDate.ToString();
				currentTask.StartDate = startTimeSelectedDate;
				if (Delegate != null)
				{
					Delegate.UpdateTaskTimelog(currentTask);
				}
				else if (DelegateforTasktimelog != null)
				{
					DelegateforTasktimelog.UpdateTaskTimelog(currentTask);
				}
				this.StartTimeText.ResignFirstResponder();
			};

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 };

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.StartTimeText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			this.StartTimeText.InputView = StartTimePicker;
			this.StartTimeText.InputAccessoryView = toolbar;

			DeltaText.Text = TimeSpan.FromMinutes(currentTask.LoggedTime).ToString(@"hh\:mm");

			IntLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

			IntText.Text = TimeSpan.FromMinutes(currentTask.InterruptTime).ToString(@"hh\:mm");

			CommentText.SetTitle(currentTask.Comment ?? "No Comment", UIControlState.Normal);

			CommentText.TouchUpInside += (sender, e) =>
			{
				
				UIAlertView alert = new UIAlertView();
				alert.Title = "Comment";
				alert.AddButton("Cancel");
				alert.AddButton("Save");
				alert.Message = "Please enter new Comment";
				alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
				UITextField textField = alert.GetTextField(0);
				textField.Placeholder = currentTask.Comment ?? "No Comment"; 
				alert.Clicked += CommentButtonClicked;
				alert.Show();
			};


			/////Delta Picker 
			DeltaPicker = new UIPickerView(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 250, this.View.Frame.Width - 20, 200f));
			DeltaPicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			DeltaPicker.UserInteractionEnabled = true;
			DeltaPicker.ShowSelectionIndicator = true;


			string[] hours = new string[24];
			string[] minutes = new string[60];

			for (int i = 0; i < hours.Length; i++)
			{
	            hours[i] = i.ToString();

			}
			for (int i = 0; i < minutes.Length; i++)
			{
				if (i < 10)
				{
					minutes[i] = "0" + i.ToString();
				}
				else {
					minutes[i] = i.ToString();
				}
			}

			StatusPickerViewModel deltaModel = new StatusPickerViewModel(hours,minutes);

			int h = (int)currentTask.LoggedTime / 60;
			int m = (int)currentTask.LoggedTime % 60;
			this.deltaSelectedHour = h.ToString();

			if (m < 10)
			{
				this.deltaSelectedMinute = "0" + m.ToString();
			}
			else {
				this.deltaSelectedMinute = m.ToString();
			}

			deltaModel.NumberSelected += (Object sender,EventArgs e) =>
			{
				this.deltaSelectedHour = deltaModel.selectedHour;
				this.deltaSelectedMinute = deltaModel.selectedMinute;

			};

			DeltaPicker.Model = deltaModel;
			DeltaPicker.BackgroundColor = UIColor.White;
			DeltaPicker.Select(h, 0, true);
			DeltaPicker.Select(m, 1, true);

			 //Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Bordered, null);

			saveButton.Clicked += (s, e) =>
			{
				
				this.DeltaText.Text = this.deltaSelectedHour + ":" + this.deltaSelectedMinute;
				currentTask.LoggedTime = int.Parse(this.deltaSelectedHour) * 60 + int.Parse(this.deltaSelectedMinute);
				if (Delegate != null)
				{
					Delegate.UpdateTaskTimelog(currentTask);
				}
				else if (DelegateforTasktimelog != null)
				{
					DelegateforTasktimelog.UpdateTaskTimelog(currentTask);
				}
				this.DeltaText.ResignFirstResponder();
			};

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.DeltaText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton}, true);

			this.DeltaText.InputView = DeltaPicker;
			this.DeltaText.InputAccessoryView = toolbar;

			////// Int Picker
			IntPicker = new UIPickerView(new CoreGraphics.CGRect(10f, this.View.Frame.Height - 200, this.View.Frame.Width - 20, 200f));
			IntPicker.BackgroundColor = UIColor.FromRGB(220, 220, 220);

			IntPicker.UserInteractionEnabled = true;
			IntPicker.ShowSelectionIndicator = true;
			IntPicker.BackgroundColor = UIColor.White;

			IntPicker.Select(0, 0, true);

			StatusPickerViewModel intModel = new StatusPickerViewModel(hours,minutes);


			int hh = (int)currentTask.InterruptTime / 60;
			int mm = (int)currentTask.InterruptTime % 60;

			this.intSelectedHour = hh.ToString();

			if (mm < 10)
			{
				this.intSelectedMinute = "0" + mm.ToString();
			}
			else {
				this.intSelectedMinute = mm.ToString();
			}

			intModel.NumberSelected += (Object sender, EventArgs e) =>
			{
				this.intSelectedHour = intModel.selectedHour;
				this.intSelectedMinute = intModel.selectedMinute;
			};

			IntPicker.Model = intModel;

			IntPicker.Select(hh, 0, true);
			IntPicker.Select(mm, 1, true);

			//Setup the toolbar
			toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.BackgroundColor = UIColor.FromRGB(220, 220, 220);
			toolbar.Translucent = true;
			toolbar.SizeToFit();


			saveButton = new UIBarButtonItem("Save", UIBarButtonItemStyle.Bordered, null);

			saveButton.Clicked += (s, e) =>
			{
				this.IntText.Text = this.intSelectedHour + ":" + this.intSelectedMinute;
				currentTask.InterruptTime = int.Parse(this.intSelectedHour) * 60 + int.Parse(this.intSelectedMinute);
				if (Delegate != null)
				{
					Delegate.UpdateTaskTimelog(currentTask);
				}
				else if (DelegateforTasktimelog != null)
				{
					DelegateforTasktimelog.UpdateTaskTimelog(currentTask);
				}

				this.IntText.ResignFirstResponder();
			};

			cancelButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Bordered,
			(s, e) =>
			{
				this.IntText.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { cancelButton, spacer, saveButton }, true);

			this.IntText.InputView = IntPicker;
			this.IntText.InputAccessoryView = toolbar;

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			TaskNameLabel.Text = currentTask.Task.FullName;
			StartTimeText.Text = currentTask.StartDate.ToLocalTime().ToString("g");
			DeltaText.Text = TimeSpan.FromMinutes(currentTask.LoggedTime).ToString(@"hh\:mm");

		}

		// this will be called before the view is displayed
		public void SetTask(TimeLogPageViewController d, TimeLogEntry task)
		{
			Delegate = d;
			currentTask = task;
		}

		public void SetTaskforTaskTimelog(TaskTimeLogViewController d, TimeLogEntry task)
		{
			DelegateforTasktimelog = d;
			currentTask = task;
		}

		public void CreateTask(TaskTimeLogViewController d, TimeLogEntry task)
		{
			DelegateforAddingTimelog = d;
			currentTask = task;
		}

	
		public void CommentButtonClicked(object sender, UIButtonEventArgs e)
		{
			UIAlertView parent_alert = (UIAlertView)sender;

			if (e.ButtonIndex == 1)
			{
				CommentText.SetTitle(parent_alert.GetTextField(0).Text, UIControlState.Normal);
				currentTask.Comment = parent_alert.GetTextField(0).Text;
				if (Delegate != null)
				{
					Delegate.UpdateTaskTimelog(currentTask);
				}
				else if (DelegateforTasktimelog != null)
				{
					DelegateforTasktimelog.UpdateTaskTimelog(currentTask);
				}
			}

		}


		public static DateTime ConvertNSDateToDateTime(NSDate date)
		{
			DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0);
			DateTime currentDate = reference.AddSeconds(date.SecondsSinceReferenceDate);
			DateTime localDate = currentDate.ToLocalTime();
			return localDate;
		}

		public static NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}
	}
}