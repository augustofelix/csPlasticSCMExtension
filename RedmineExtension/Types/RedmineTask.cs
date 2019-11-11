using System;

using Redmine.Net.Api.Types;

namespace Codice.Client.Extension.Types
{
    public class RedmineTask : PlasticTask
    {
        public RedmineTask(Issue issue)
        {
            this.Id = Convert.ToString(issue.Id);
            this.Title = issue.Subject;
            this.Description = issue.Description;
            this.Status = issue.Status.Name;
            this.Owner = issue.AssignedTo.Name;
        }

        public RedmineTask(Issue issue, string repName) : this(issue)
        {
            this.RepName = repName;
        }
    }
}
