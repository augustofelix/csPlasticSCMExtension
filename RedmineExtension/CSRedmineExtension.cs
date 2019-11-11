using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using log4net;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Codice.Utils;

namespace Codice.Client.IssueTracker.RedmineExtension
{
    public class CSRedmineExtension : IPlasticIssueTrackerExtension
    {
       
        public IssueTrackerConfiguration mConfig;

        
        public static string ConfigFileName = "redmineextension.conf";

        public static readonly string ExtensionName = "Redmine Extension";
        public static readonly string USER_KEY = "User";
        public static readonly string PASSWORD_KEY = "Password";
        public static readonly string HOST_KEY = "Server";
        public static readonly string BRANCH_PREFIX_KEY = "Branch prefix";
        public static readonly string APIKEY_KEY = "API Key";
        public static readonly string PENDING_TASK_KEY = "Pending Tasks State";
        public static readonly string OPEN_TASK_KEY = "Open Tasks State";
        public static readonly string CLOSE_TASK_KEY = "Close Tasks State";

        private static readonly ILog mLog = LogManager.GetLogger("extensions");

        private string pageSize = "50";

        public string PageSize { get => pageSize; set => pageSize = value; }

        // If you want logging, this is the way to declare it
        // Don't forget to set log4net.dll as a reference!
        //static readonly ILog mLog = LogManager.GetLogger("redmineextension");

        //public RedmineExtension2() { }

        internal CSRedmineExtension(IssueTrackerConfiguration config)
        {
            mConfig = config;
            
            mLog.Info("CSRedmineExtension issue tracker is initialized");
        }

        public void Connect()
        {
            // Is not necessary.
        }

        public void Disconnect()
        {
            // Is not necessary.
        }

        public string GetExtensionName()
        {
            return "Redmine Issues";
        }

        public List<PlasticTask> GetPendingTasks()
        {
            try
            {
                return GetPendingTasks("");
            }
            catch(Exception)
            {
                throw;
            }
            
        }

        public List<PlasticTask> GetPendingTasks(string assignee)
        {
            try
            {
                string pendingState = mConfig.GetValue(CSRedmineExtension.PENDING_TASK_KEY).ToLower();
                IList<Issue> issues = null;
                NameValueCollection parameters = new NameValueCollection();
                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);
                
                parameters.Set("limit", PageSize);
                issues = redmineManager.GetObjectList<Issue>(parameters, out int count);
                List<PlasticTask> plasticTasks = new List<PlasticTask>(count);

                User user = redmineManager.GetCurrentUser();

                if (assignee.ToLower() == user.Login.ToLower())
                {
                    for (int i = 0; i < issues.Count; i++)
                    {
                        if (issues[i].Status.Name.ToLower() == pendingState && issues[i].Author.Name.ToLower() == user.FirstName.ToLower())
                        {
                            PlasticTask plasticTask = new PlasticTask
                            {
                                Description = issues[i].Description,
                                Owner = issues[i].Author.Name,
                                Id = issues[i].Id.ToString(),
                                Status = issues[i].Status.Name,
                                Title = issues[i].Subject,

                                CanBeLinked = true //??
                            };

                            plasticTasks.Add(plasticTask);
                        }
                    }
                    return plasticTasks;
                }
                else if (string.IsNullOrEmpty(assignee))
                {
                    for (int i = 0; i < issues.Count; i++)
                    {
                        if (issues[i].Status.Name.ToLower() == pendingState)
                        {
                            PlasticTask plasticTask = new PlasticTask
                            {
                                Description = issues[i].Description,
                                Owner = issues[i].Author.Name,
                                Id = issues[i].Id.ToString(),
                                Status = issues[i].Status.Name,
                                Title = issues[i].Subject,
                                //plasticTask.RepName = ??
                                CanBeLinked = true //??
                            };

                            plasticTasks.Add(plasticTask);
                        }
                    }
                }

                return plasticTasks;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public PlasticTask GetTaskForBranch(string fullBranchName)
        {
            try
            {
                string taskId = "";
                taskId = GetTaskIdFromBranchName(GetBranchName(fullBranchName));
                PlasticTask plasticTask = LoadTaskId(taskId);
                return plasticTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<string, PlasticTask> GetTasksForBranches(List<string> fullBranchNames)
        {
            Dictionary<string, PlasticTask> taskForBranches = new Dictionary<string, PlasticTask>();

            try
           {
                PlasticTask plasticTask = new PlasticTask();
                IList<Issue> issues = null;
                NameValueCollection parameters = new NameValueCollection();
                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);
                parameters.Set("limit", pageSize);
                issues = redmineManager.GetObjectList<Issue>(parameters, out int count);
                
                foreach(string brunchName in fullBranchNames)
                {
                    plasticTask = GetTaskForBranch(brunchName);
                    taskForBranches.Add(GetBranchName(brunchName), plasticTask);
                }

            }
            catch(Exception)
            {
                throw;
            }

             return taskForBranches;
        }

        public PlasticTask LoadTaskId(string taskId)
        {
            try
            {
                List<string> taskIds = new List<string>
                {
                    taskId
                };
                List<PlasticTask> plasticTasks = new List<PlasticTask>();
                plasticTasks = LoadTasks(taskIds);
                return plasticTasks[0];
            }
            catch(Exception)
            {
                throw;
            }
        }

        public List<PlasticTask> LoadTasks(List<string> taskIds)
        {
            try

            {
                List<PlasticTask> taskList = new List<PlasticTask>();
                
                Issue issue;

                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);

                for (int i = 0; i < taskIds.Count; i++)
                {
                    PlasticTask plasticTask = new PlasticTask();

                    if (!string.IsNullOrEmpty(taskIds[i]))
                    {
                        NameValueCollection nameValueCollection = new NameValueCollection
                        {
                            { "include", "journals,changesets" }
                        };
                        issue = redmineManager.GetObject<Issue>(taskIds[i], nameValueCollection);
                        plasticTask.Description = issue.Description;
                        plasticTask.Owner = issue.Author.Name;
                        plasticTask.Id = issue.Id.ToString();
                        plasticTask.Status = issue.Status.Name;
                        plasticTask.Title = issue.Subject;
                        //plasticTask.RepName = fullBranchNames[4];
                        plasticTask.CanBeLinked = true; //??
                    }
                    else
                    {
                        plasticTask = null;
                    }

                    taskList.Add(plasticTask);
                }

                return taskList;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void LogCheckinResult(PlasticChangeset changeset, List<PlasticTask> tasks)
        {
            string msgLog = "Updating changeset: " + changeset.Id.ToString();

            try
            {
                msgLog = msgLog + " with Redmine Task: " + string.Join(",", tasks);
                mLog.Info(msgLog);
                
            }
            catch(Exception oEx)
            {
                mLog.Error(msgLog,oEx);
                throw;
            }
        }

        public void MarkTaskAsOpen(string taskId, string assignee)
        {
            try
            {
                IList<Journal> journals = new List<Journal>();
                Journal journal = new Journal();

                NameValueCollection nameValueCollection = new NameValueCollection
                {
                    { "include", "journals,changesets" }
                };

                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);
                Issue issue = new Issue();
                IssueStatus issueStatus = new IssueStatus();

                string issueStatusName = mConfig.GetValue(CSRedmineExtension.OPEN_TASK_KEY);

                issue = redmineManager.GetObject<Issue>(taskId, nameValueCollection);

                issueStatus.Name = issueStatusName;
                issueStatus.Id = GetIdStatus(issueStatusName);
                issue.Status = issueStatus;
                issue.EstimatedHours = null;

                journal.Notes = "Updated from Plastic SCM" + "\n" + "Branch: " + mConfig.GetValue(BRANCH_PREFIX_KEY) + issue.Id.ToString().Trim();
                journals.Add(journal);
                issue.Journals = journals;

                redmineManager.UpdateObject(taskId, issue);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void OpenTaskExternally(string taskId)
        {
            try
            {
                string host = mConfig.GetValue(CSRedmineExtension.HOST_KEY);
                System.Diagnostics.Process.Start(host + "/issues/" + taskId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool TestConnection(IssueTrackerConfiguration configuration)
        {

            try
            {
                CSRedmineTestConnection testConnection = new CSRedmineTestConnection();
                return testConnection.CsTestConnection(configuration);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void UpdateLinkedTasksToChangeset(PlasticChangeset changeset, List<string> tasks)
        {

            try
            {

                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);
                Issue issue = new Issue();
                IssueStatus issueStatus = new IssueStatus();

                string issueStatusName = mConfig.GetValue(CSRedmineExtension.OPEN_TASK_KEY);
                string strNote;

                NameValueCollection nameValueCollection = new NameValueCollection
                {
                    { "include", "journals,changesets" }
                };

                for (int i = 0; i < tasks.Count; i++)
                {
                    IList<Journal> journals = new List<Journal>();
                    Journal journal = new Journal();
                    strNote = "";

                    issue = redmineManager.GetObject<Issue>(tasks[i], nameValueCollection);

                    issueStatus.Name = issueStatusName;
                    issueStatus.Id = GetIdStatus(issueStatusName);
                    issue.Status = issueStatus;
                    issue.EstimatedHours = null;

                    if (!ChkIssueNote(issue.Journals, changeset.Id))
                    {
                        strNote = "Update from Plastic SCM:\n" + changeset.Branch + "\n" + changeset.Comment + "\n";
                        strNote = strNote + "Revision: r" + Convert.ToString(changeset.Id).Trim();
                        journal.Notes = strNote;
                        journals.Add(journal);
                        issue.Journals = journals;
                    }
                    redmineManager.UpdateObject(tasks[i], issue);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }



        #region Helpers

        /// <summary>
        /// Create a Redmine Manager
        /// </summary>
        /// <param name="repName">It is the HOST of Redmine, Ip or Url</param>
        /// <returns>RedmineManager Object</returns>
        private RedmineManager CreateRedmineManager(string repName)
        {
            try
            {
                string host = mConfig.GetValue(repName);
                string apikey = mConfig.GetValue(CSRedmineExtension.APIKEY_KEY);

                if (apikey == null || apikey == String.Empty)
                {
                    string user = mConfig.GetValue(CSRedmineExtension.USER_KEY);
                    string password = CryptoServices.GetDecryptedPassword(mConfig.GetValue(CSRedmineExtension.PASSWORD_KEY));    //GetPassword(repName)

                    return new RedmineManager(host, user, password);
                }

                return new RedmineManager(host, apikey);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the last part of branch.
        /// </summary>
        /// <param name="fullBranchName">Full Branch Name when click on in Plastic.</param>
        /// <returns>Last part of branch.</returns>
        string GetBranchName(string fullBranchName)
        {
            int lastSeparatorIndex = fullBranchName.LastIndexOf('/');

            if (lastSeparatorIndex < 0)
                return fullBranchName;

            if (lastSeparatorIndex == fullBranchName.Length - 1)
                return string.Empty;

            return fullBranchName.Substring(lastSeparatorIndex + 1);
        }

        /// <summary>
        /// Get ID of task of a given branch.
        /// </summary>
        /// <param name="branchName">Branch name, obtained of GetBranchName</param>
        /// <returns>TaskId</returns>
        string GetTaskIdFromBranchName(string branchName)
        {
            string prefix = mConfig.GetValue(CSRedmineExtension.BRANCH_PREFIX_KEY);
            if (string.IsNullOrEmpty(prefix))
                return branchName;

            if (!branchName.StartsWith(prefix) || branchName == prefix)
                return string.Empty;

            return branchName.Substring(prefix.Length).Trim();
        }

        /// <summary>
        /// Get the ID of task status
        /// </summary>
        /// <param name="statusName">Name of task status</param>
        /// <returns>Id</returns>
        int GetIdStatus(string statusName)
        {
            try
            {
                int statusId = 0;
                IList<IssueStatus> issueStatuses = null;
                RedmineManager redmineManager = CreateRedmineManager(CSRedmineExtension.HOST_KEY);
                issueStatuses = redmineManager.GetObjectList<IssueStatus>(null, out int count);

                foreach (IssueStatus status in issueStatuses)
                {
                    if (status.Name.ToLower() == statusName.ToLower())
                    {
                        statusId = status.Id;
                    }
                }

                return statusId;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Search revision link into the note.
        /// </summary>
        /// <param name="journals"></param>
        /// <param name="changesetId"></param>
        /// <returns>True, if found it.</returns>
        private bool ChkIssueNote(IList<Journal> journals, long changesetId)
        {
            try
            {
                if (journals == null)
                {
                    return false;
                }

                string searchId = "";
                bool breturn = false;
                searchId = "r" + Convert.ToString(changesetId).Trim();

                for (int i = 0; i < journals.Count; i++)
                {
                    breturn = breturn || journals[i].Notes.IndexOf(searchId) >= 0;
                }
                return breturn;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }

    public class CSRedmineExtensionFactory : IPlasticIssueTrackerExtensionFactory
    {
        public CSRedmineExtensionFactory()
        {
        }

        public IssueTrackerConfiguration GetConfiguration(IssueTrackerConfiguration storedConfiguration)
        {
            List<IssueTrackerConfigurationParameter> parameters = new List<IssueTrackerConfigurationParameter>();

            ExtensionWorkingMode workingMode = GetWorkingMode(storedConfiguration);

            string user = GetValidParameterValue(storedConfiguration, CSRedmineExtension.USER_KEY, "");

            string prefix = GetValidParameterValue(storedConfiguration, CSRedmineExtension.BRANCH_PREFIX_KEY, "sample");

            IssueTrackerConfigurationParameter userIdParam =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.USER_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.USER_KEY, ""),
                    Type = IssueTrackerConfigurationParameterType.User,
                    IsGlobal = false
                };

            IssueTrackerConfigurationParameter branchPrefixParam =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.BRANCH_PREFIX_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.BRANCH_PREFIX_KEY, "scm"),
                    Type = IssueTrackerConfigurationParameterType.BranchPrefix,
                    IsGlobal = true
                };

            IssueTrackerConfigurationParameter passwordParam =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.PASSWORD_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.PASSWORD_KEY, ""),
                    Type = IssueTrackerConfigurationParameterType.Password,
                    IsGlobal = false
                };

            IssueTrackerConfigurationParameter hostParam =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.HOST_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.HOST_KEY, ""),
                    Type = IssueTrackerConfigurationParameterType.Host,
                    IsGlobal = true
                };

            IssueTrackerConfigurationParameter apiKeyParam =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.APIKEY_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.APIKEY_KEY, ""),
                    Type = IssueTrackerConfigurationParameterType.Text,
                    IsGlobal = false
                };

            IssueTrackerConfigurationParameter pendingTask =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.PENDING_TASK_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.PENDING_TASK_KEY, "Pending"),
                    Type = IssueTrackerConfigurationParameterType.Text,
                    IsGlobal = true
                };

            IssueTrackerConfigurationParameter openTask =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.OPEN_TASK_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.OPEN_TASK_KEY, "Open"),
                    Type = IssueTrackerConfigurationParameterType.Text,
                    IsGlobal = true
                };

            IssueTrackerConfigurationParameter closeTask =
                new IssueTrackerConfigurationParameter()
                {
                    Name = CSRedmineExtension.CLOSE_TASK_KEY,
                    Value = GetValidParameterValue(storedConfiguration, CSRedmineExtension.CLOSE_TASK_KEY, "Close"),
                    Type = IssueTrackerConfigurationParameterType.Text,
                    IsGlobal = true
                };

            parameters.Add(hostParam);
            parameters.Add(userIdParam);
            parameters.Add(passwordParam);
            parameters.Add(branchPrefixParam);
            parameters.Add(pendingTask);
            parameters.Add(openTask);
            //parameters.Add(closeTask);    It is no use, because no action are implemented in Plastic, for this condition.
            parameters.Add(apiKeyParam);

            return new IssueTrackerConfiguration(workingMode, parameters);

        }
        ExtensionWorkingMode GetWorkingMode(IssueTrackerConfiguration config)
        {
            if (config == null)
                return ExtensionWorkingMode.TaskOnBranch;

            if (config.WorkingMode == ExtensionWorkingMode.None)
                return ExtensionWorkingMode.TaskOnBranch;

            return config.WorkingMode;
        }

        string GetValidParameterValue(IssueTrackerConfiguration config, string paramName, string defaultValue)
        {
            string configValue = config?.GetValue(paramName);
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }


        public IPlasticIssueTrackerExtension GetIssueTrackerExtension(IssueTrackerConfiguration configuration)
        {
            try
            {
                return new CSRedmineExtension(configuration);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetIssueTrackerName()
        {
            return CSRedmineExtension.ExtensionName;
        }
    }
}
