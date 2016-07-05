﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using ProcessDashboard.Model;
using System.Collections;

namespace ProcessDashboard.DBWrapper
{
    public class TimeLogWrapper
    {
        SQLiteConnection db;

        public TimeLogWrapper(SQLiteConnection db)
        {
            this.db = db;
        }

        public bool createTable()
        {
            try
            {
                db.CreateTable<TimeLogEntryModel>();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool insertMultipleRecords(List<TimeLogEntryModel> entries)
        {
            db.RunInTransaction(() =>
            {
                // database calls inside the transaction
                foreach (TimeLogEntryModel tem in entries)
                {
                    db.Insert(tem);
                }

            });
            return true;
        }

        public bool insertRecord(TimeLogEntryModel timelogentry)
        {
            try
            {
                db.Insert(timelogentry);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public List<TimeLogEntryModel> GetAllRecords()
        {
            try
            {
                var table = db.Table<TimeLogEntryModel>().ToList();

                return table;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public TimeLogEntryModel getRecord(string RowId)
        {
            try
            {
                var item = db.Get<TimeLogEntryModel>(RowId);
                return item;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public bool updateRecord(string RowId, Hashtable ht)
        {
            try
            {
                var item = db.Get<TimeLogEntryModel>(RowId);

                if (ht.ContainsKey("TimeLogId"))
                {
                    item.TimeLogId = ht["TimeLogId"].ToString();
                }
                if (ht.ContainsKey("TaskId"))
                {
                    item.TaskId = ht["TaskId"].ToString();
                }
                if (ht.ContainsKey("StartDate"))
                {
                    item.StartDate = DateTime.Parse(ht["StartDate"].ToString());
                }
                if (ht.ContainsKey("ElapsedTime"))
                {
                    //Change has happened to an elapsed time
                    //TODO: Add elapsed thingy
                    item.ElapsedTime = double.Parse(ht["ElapsedTime"].ToString());
                }
                if (ht.ContainsKey("InterruptTime"))
                {
                    item.InterruptTime = double.Parse(ht["InterruptTime"].ToString());
                }
                if (ht.ContainsKey("Comment"))
                {
                    item.Comment = (ht["Comment"].ToString());
                }
                if (ht.ContainsKey("IsOpen"))
                {
                    item.IsOpen = bool.Parse(ht["IsOpen"].ToString());
                }
                if (ht.ContainsKey("ChangeFlag"))
                {
                    item.ChangeFlag = (ht["ChangeFlag"].ToString()[0]);
                }
                item.EditTimestamp = DateTime.Now;
                db.Update(item);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool deleteRecord(string RowId)
        {
            try
            {
                db.Delete<TimeLogEntryModel>(RowId);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }

        }
    }
}