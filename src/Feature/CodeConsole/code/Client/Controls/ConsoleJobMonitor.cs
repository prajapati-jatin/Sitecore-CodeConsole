using Sitecore.Diagnostics;
using Sitecore.Feature.CodeConsole.Client.Applications;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Jobs.AsyncUI;
using Sitecore.Security.Accounts;
using Sitecore.Sites;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Controls
{
    public class ConsoleJobMonitor : Control
    {
        public Boolean Active
        {
            get
            {
                return base.GetViewStateBool("active", true);
            }
            set
            {
                base.SetViewStateBool("active", value);
                if (value)
                {
                    this.ScheduleCallback();
                }
            }
        }

        public Handle JobHandle
        {
            get
            {
                String viewStateString = base.GetViewStateString("task");
                return (!String.IsNullOrEmpty(viewStateString) ? Handle.Parse(viewStateString) : Handle.Null);
            }
            set
            {
                base.SetViewStateString("task", value.ToString());
            }
        }

        public String SessionID
        {
            get
            {
                return base.GetViewStateString("taskMonitorSessionId");
            }
            set
            {
                base.SetViewStateString("taskMonitorSessionId", value);
            }
        }

        public ConsoleJobMonitor()
        {

        }

        public override void HandleMessage(Message message)
        {
            IMessage message1;
            base.HandleMessage(message);
            if(message.Name == "consoletaskmonitor:check")
            {
                Handle jobHandle = this.JobHandle;
                if (!jobHandle.IsLocal)
                {
                    Job job = Sitecore.Jobs.JobManager.GetJob(jobHandle);
                    if(job != null)
                    {
                        while(job.MessageQueue.GetMessage(out message1))
                        {
                            message1.Execute();
                            CompleteMessage completeMessage = message1 as CompleteMessage;
                            if(completeMessage != null)
                            {
                                this.OnJobFinished(completeMessage.RunnerOutput);
                                return;
                            }
                        }
                        this.ScheduleCallback();
                    }
                    else
                    {
                        this.OnJobDisappeared();
                    }
                }
                else
                {
                    this.ScheduleCallback();
                }
            }
        }

        public void Start(String name, String category, ThreadStart task, Language language = null, User user = null, JobOptions options = null)
        {
            String empty = String.Empty;
            String userName = String.Empty;
            Assert.ArgumentNotNullOrEmpty(name, "name");
            Assert.ArgumentNotNullOrEmpty(category, "category");
            Assert.ArgumentNotNull(task, "task");
            SiteContext site = Sitecore.Context.Site;
            if(site != null)
            {
                empty = site.Name;
            }
            if(Sitecore.Context.User != null)
            {
                userName = Sitecore.Context.User.Name;
            }

            JobOptions jobOptions = new JobOptions(String.Format("{0} - {1}", name, userName), category, empty, new ConsoleJobMonitor.TaskRunner(task), "Run");
            object contextUser = user;
            if(contextUser == null)
            {
                if(options != null)
                {
                    contextUser = options.ContextUser;
                }
                else
                {
                    contextUser = null;
                }
                if(contextUser == null)
                {
                    contextUser = Context.User;
                }
            }
            jobOptions.ContextUser = (User)contextUser;
            jobOptions.AtomicExecution = false;
            jobOptions.EnableSecurity = (options != null) ? options.EnableSecurity : true;
            object clientLanguage = language;
            if(clientLanguage == null)
            {
                if(options != null)
                {
                    clientLanguage = options.ClientLanguage;
                }
                else
                {
                    clientLanguage = null;
                }
                if(clientLanguage == null)
                {
                    clientLanguage = Sitecore.Context.Language;
                }
            }
            jobOptions.ClientLanguage = (Language)clientLanguage;
            jobOptions.AfterLife = new TimeSpan(0, 0, 0, 10);
            this.JobHandle = JobManager.Start(jobOptions).Handle;
            this.ScheduleCallback();
        }

        private void OnJobFinished(RunnerOutput runnerOutput)
        {
            this.JobHandle = Handle.Null;
            SessionCompleteEventArgs sessionCompleteEventArgs = new SessionCompleteEventArgs()
            {
                RunnerOutput = runnerOutput
            };
            EventHandler eventHandler = this.JobFinished;
            if(eventHandler != null)
            {
                eventHandler(this, sessionCompleteEventArgs);
            }
        }

        public event EventHandler JobDisappeared;
        public event EventHandler JobFinished;

        private void OnJobDisappeared()
        {
            this.JobHandle = Handle.Null;
            EventHandler eventHandler = this.JobDisappeared;
            if(eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        private void ScheduleCallback()
        {
            if (this.Active)
            {
                SheerResponse.Timer("consoletaskmonitor:check", 500);
            }
        }

        public class TaskRunner
        {
            private readonly ThreadStart task;

            public TaskRunner(ThreadStart task)
            {
                Assert.ArgumentNotNull(task, nameof(task));
                this.task = task;
            }

            public void Run()
            {
                this.task();
            }
        }
    }
}