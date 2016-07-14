﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessDashboard.Service.Interface;
using Fusillade;
using ProcessDashboard.DBWrapper;
using ProcessDashboard.DTO;

namespace ProcessDashboard.SyncLogic
{
    public class Controller
    {
        /*
   * 
   * Name: Controller.cs
   * 
   * Purpose: This class is used by the iOS/Android app to get the data.
   * 
   * Description:
   * This class provides methods which can be used by the iOS/Android app. The methods return the list of tasks/projects/timelog entries as well as individual entries.
   * It also abstracts the difference between data obtained from the server and data obtained from the database.

   */
        
        private readonly IPDashServices _pDashServices;

        private readonly DBManager _dbm;

        public Controller(IPDashServices pDash)
        {
            this._pDashServices = pDash;
            _dbm = DBManager.getInstance();

        }
        /*
            if (CrossConnectivity.Current.IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("TaskModel Service : " + " Setting connection policy");
                task = await Policy
                    .Handle<Exception>()
                    .RetryAsync(retryCount: 5)
                    .ExecuteAsync(async () => await getTaskDtoTask);
            }
         */

        // Get list of projects
        public async Task<List<Project>> GetProjects(string dataset)
        {
            //var localprojects = await _pDashServices.GetProjectsListLocal(Priority.UserInitiated, dataset).ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine("We are in the project list task");

            var remoteProjects = await _pDashServices.GetProjectsListRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);

            return remoteProjects;

            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<Project> entries;

            if (localprojects == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteProjects;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                // Convert the remote data into a DB storable form
                List<ProjectModel> models = Mapper.GetInstance().toProjectModelList(entries);
                // Store the remote data into the DB
                _dbm.pw.insertMultipleRecords(models);

            }
            else
            {
                entries = localprojects;
            }

            // Do any UI Operations
            return entries;
            */
        }

        // Get list of tasks
        public async Task<List<DTO.Task>> GetTasks(string dataset, string projectID)
        {
            //var localTasks = await _pDashServices
            //                              .GetTasksListLocal(Priority.UserInitiated, dataset,projectID)
            //                            .ConfigureAwait(false);

            var remoteTasks = await _pDashServices.GetTasksListRemote(Priority.UserInitiated, dataset, projectID)
                .ConfigureAwait(false);
            return remoteTasks;
            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<DTO.Task> entries;

            if (localTasks == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteTasks;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                 
                System.Diagnostics.Debug.WriteLine("Controller : " + "Going for mapping " + (entries == null));
                // Convert the remote data into a DB storable form
                List<TaskModel> models = Mapper.GetInstance().toTaskModelList(entries);
                System.Diagnostics.Debug.WriteLine("Controller : "+"Done with the mapping "+(models==null));
                // Store the remote data into the DB
                _dbm.tw.insertMultipleRecords(models);
                System.Diagnostics.Debug.WriteLine("Controller : " + "Done with the inserting into DB");

            }
            else
            {
                entries = localTasks;
            }

            // Do any UI Operations
            return entries;
            */
        }

        // Get Time Log entries with optional parameters.
        // Optional parameters should be specified as null
        public async Task<List<TimeLogEntry>> GetTimeLog(string dataset,int maxResults, string startDateFrom, string startDateTo, string taskId, string projectId)
        {

            var remoteTasks = await _pDashServices.GetTimeLogsRemote(Priority.UserInitiated, dataset,maxResults,startDateFrom,startDateTo,taskId,projectId)
                .ConfigureAwait(false);
            return remoteTasks;

        }
        
        // Get the list of recent tasks
        public async Task<List<DTO.Task>> GetRecentTasks(string dataset)
        {
            //var localTasks = await _pDashServices
            //                              .GetTasksListLocal(Priority.UserInitiated, dataset,projectID)
            //                            .ConfigureAwait(false);

            var remoteTasks = await _pDashServices.GetRecentTasksRemote(Priority.UserInitiated, dataset)
                .ConfigureAwait(false);
            return remoteTasks;
            /*
            // TODO: Bind the values returned by remoteProjects or use a mechanism that executes after remoteProjects is obtained.

            List<DTO.Task> entries;

            if (localTasks == null)
            {
                // We need to wait till we get the remote data.
                entries = await remoteTasks;
                // Use our sync logic to validate the data received
                //TODO: Check Sync logic

                 
                System.Diagnostics.Debug.WriteLine("Controller : " + "Going for mapping " + (entries == null));
                // Convert the remote data into a DB storable form
                List<TaskModel> models = Mapper.GetInstance().toTaskModelList(entries);
                System.Diagnostics.Debug.WriteLine("Controller : "+"Done with the mapping "+(models==null));
                // Store the remote data into the DB
                _dbm.tw.insertMultipleRecords(models);
                System.Diagnostics.Debug.WriteLine("Controller : " + "Done with the inserting into DB");

            }
            else
            {
                entries = localTasks;
            }

            // Do any UI Operations
            return entries;
            */

        }

        public async Task<DTO.Task> GetTask(string dataset, string taskId)
        {
            var remoteTasks = await _pDashServices.GetTaskDetailsRemote(Priority.UserInitiated, dataset,taskId)
             .ConfigureAwait(false);
            return remoteTasks;
        }

        /******* TEST CODE ****/

        public async void testProject()
        {
            
            List<Project> projectsList = await GetProjects("mock");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF PROJECTS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.name))
                {
                    System.Diagnostics.Debug.WriteLine(proj);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void testTasks()
        {

            List<DTO.Task> projectsList = await GetTasks("mock", "iokdum2d");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.fullName))
                {
                    System.Diagnostics.Debug.WriteLine(proj);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }



        }

        public async void testTaskProject()
        {

            List<DTO.Task> projectsList = await GetTasks("mock", "iokdum2d");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.project))
                {
                    System.Diagnostics.Debug.WriteLine(proj.name);
                    
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }



        }

        public async void testTimeLog()
        {
            List<TimeLogEntry> timeLogEntries = await GetTimeLog("mock",0,null,null,null,null);
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
                System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var proj in timeLogEntries)
                {
                    System.Diagnostics.Debug.WriteLine("Task Name : " + proj.task.fullName);
                    System.Diagnostics.Debug.WriteLine("Start Date : "+proj.startDate);
                    System.Diagnostics.Debug.WriteLine("End Date : " + proj.endDate);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

        public async void testTimeLogWithID(string taskID)
        {
            List<TimeLogEntry> timeLogEntries = await GetTimeLog("mock", 0, null, null, taskID,null);
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF Timelog **");
                System.Diagnostics.Debug.WriteLine("Length is " + timeLogEntries.Count);

                foreach (var proj in timeLogEntries)
                {
                    System.Diagnostics.Debug.WriteLine("Task Name : " + proj.task.fullName);
                    System.Diagnostics.Debug.WriteLine("Start Date : " + proj.startDate);
                    System.Diagnostics.Debug.WriteLine("End Date : " + proj.endDate);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }
        }

        public async void testRecentTasks()
        {
            List<DTO.Task> projectsList = await GetRecentTasks("mock");
            try
            {
                System.Diagnostics.Debug.WriteLine("** LIST OF RECENT TASKS **");
                System.Diagnostics.Debug.WriteLine("Length is " + projectsList.Count);

                foreach (var proj in projectsList.Select(x => x.fullName))
                {
                    System.Diagnostics.Debug.WriteLine(proj);
                    //  _taskService.GetTasksList(Priority.Speculative, "mock", taskID);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("We are in an error state :" + e);
            }

        }

    }
}