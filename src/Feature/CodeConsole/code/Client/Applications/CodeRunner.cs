using Sitecore.Diagnostics;
using Sitecore.Feature.CodeConsole.Client.Applications;
using Sitecore.Feature.CodeConsole.Client.Controls;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Jobs.AsyncUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Applications
{
    public class CodeRunner
    {
        private Boolean AutoDispose { get; set; }

        public CodeRunner.CodeRunnerMethod Method { get; private set; }

        public String Code { get; private set; }

        public object Session { get; private set; }

        public CodeRunner(CodeRunner.CodeRunnerMethod method, object session, String code, Boolean autoDispose)
        {
            Assert.ArgumentNotNull(method, "method");
            Assert.ArgumentNotNull(code, "code");
            this.Method = method;
            this.Code = code;
            this.Session = session;
        }

        public void Run()
        {
            object clientLanguage = null;
            try
            {
                try
                {
                    Job job = Sitecore.Context.Job;
                    if (job != null)
                    {
                        JobOptions options = job.Options;
                        if (options != null)
                        {
                            clientLanguage = options.ClientLanguage;
                        }
                        else
                        {
                            clientLanguage = null;
                        }
                    }
                    if (clientLanguage == null)
                    {
                        clientLanguage = Sitecore.Context.Language;
                    }
                    Context.Language = (Language)clientLanguage;
                    this.Method(this.Session, this.Code);
                    if (Context.Job != null)
                    {
                        RunnerOutput runnerOutput = new RunnerOutput()
                        {
                            Exception = null,
                            Output = null, //TODO: Implement
                            HasErrors = false, //TODO: Implement
                        };
                        Context.Job.Status.Result = runnerOutput;
                        CompleteMessage completeMessage = new CompleteMessage()
                        {
                            RunnerOutput = runnerOutput
                        };
                        JobContext.MessageQueue.PutMessage(completeMessage);
                    }
                }
                catch (ThreadAbortException threadAbortExcpetion)
                {
                    if (!Environment.HasShutdownStarted)
                    {
                        Thread.ResetAbort();
                    }
                }
                catch (Exception ex)
                {
                    if (Context.Job != null)
                    {
                        RunnerOutput runnerOutput = new RunnerOutput()
                        {
                            Exception = ex,
                            Output = null,
                            HasErrors = false
                        };
                        Context.Job.Status.Result = runnerOutput;
                        CompleteMessage completeMessage = new CompleteMessage()
                        {
                            RunnerOutput = runnerOutput
                        };
                        JobContext.MessageQueue.PutMessage(completeMessage);
                    }
                }
            }
            finally {

            }
        }

        public delegate void CodeRunnerMethod(object session, String code);
    }
}