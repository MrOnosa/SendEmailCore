// Copyright(c) Tyler Palesano
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using McMaster.Extensions.CommandLineUtils;

namespace SendEmailCore
{
    [VersionOption("1.0.0")]
    [Command(Name = "sendemailcore", Description = "A simple command line SMTP email client")]
    [HelpOption("-?|--help")]
    public class Program
    {
        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Required]
        [EmailAddress]
        [Option(CommandOptionType.SingleValue, Description = "The email address of the sender. Required", ShortName = "f")]
        public string From { get; }

        [EmailAddress]
        [Option(CommandOptionType.MultipleValue, Description = "The email address of the receiver(s)", ShortName = "t")]
        public string[] To { get; } = new string[0];

        [Option("-s|--subject <SUBJECT>", "Message subject", CommandOptionType.SingleValue)]
        public string Subject { get; }

        [Option(CommandOptionType.SingleValue, Description = "Message body. STDIN could be used instead", ShortName = "m")]
        public string Message { get; }

        [Option(CommandOptionType.SingleValue, Description = "SMTP client host, default is localhost", ShortName = "h")]
        public string Host { get; } = "localhost";

        [Option(CommandOptionType.SingleValue, Description = "SMTP client port, default is 25", ShortName = "p")]
        public int Port { get; } = 25;

        [EmailAddress]
        [Option(CommandOptionType.MultipleValue, Description = "The email address of the cc receiver(s)", ShortName = "cc")]
        public string[] Cc { get; } = new string[0];

        [EmailAddress]
        [Option(CommandOptionType.MultipleValue, Description = "The email address of the bcc receiver(s)", ShortName = "bcc")]
        public string[] Bcc { get; } = new string[0];

        [Option(CommandOptionType.SingleValue, Description = "Username to use for authentication, like an email address", ShortName = "xu")]
        public string Username { get; }

        [Option(CommandOptionType.SingleValue, Description = "Password to use for authentication", ShortName = "xp")]
        public string Password { get; }

        [Option(CommandOptionType.SingleValue, ShortName ="T", Description = "See trace messages. --trace will display info level messages. --trade:verbose will display verbose and info level messages")]
        public (bool HasValue, TraceLevel level) Trace { get; }

        [Option(CommandOptionType.NoValue, ShortName = "", Description = "Specify the SMTP email client should not use Secure Sockets Layer (SSL) to encrypt the connection")]
        public bool DisableSsl { get; }

        private int OnExecute(CommandLineApplication app)
        {
            LogTrace(TraceLevel.Verbose, "Starting sendemailcore. Validating receiver...");
            if (To.Length == 0 && Cc.Length == 0 && Bcc.Length == 0)
            {
                Console.Out.WriteLine($"Specify at least one receiver using --{nameof(To).ToLowerInvariant()}, --{nameof(Cc).ToLowerInvariant()}, or --{nameof(Bcc).ToLowerInvariant()}.");
                app.ShowHelp();
                return 1;
            }
            string message = Message;
            if (Console.IsInputRedirected && string.IsNullOrEmpty(message))
            {
                LogTrace(TraceLevel.Info, "Message sourced from STDIN pipeline.");
                LogTrace(TraceLevel.Verbose, $"Specify --{nameof(Message).ToLowerInvariant()} to supply the message instead. Opening stream...");
                using (Stream s = Console.OpenStandardInput())
                {
                    LogTrace(TraceLevel.Verbose, "Stream opened...");
                    StreamReader reader = new StreamReader(s);
                    message = reader.ReadToEnd();
                }
                LogTrace(TraceLevel.Verbose, "Stream closed.");
            }
            LogTrace(TraceLevel.Verbose, "Validating message...");
            if (string.IsNullOrEmpty(message))
            {
                Console.Out.WriteLine($"Message body required from either STDIN or --{nameof(Message).ToLowerInvariant()}.");
                app.ShowHelp();
                return 1;
            }

            LogTrace(TraceLevel.Verbose, "Creating SmtpClient...");
            var client = new SmtpClient(Host, Port)
            {
                EnableSsl = !DisableSsl
            };
            if (!string.IsNullOrWhiteSpace(Username))
            {
                LogTrace(TraceLevel.Verbose, "Using network credentials...");
                client.Credentials = new NetworkCredential(Username, Password);
            }

            LogTrace(TraceLevel.Verbose, "Creating MailMessage...");
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(From);
            mailMessage.Body = message;
            foreach (var t in To)
            {
                LogTrace(TraceLevel.Verbose, $"Adding {t} to To...");
                mailMessage.To.Add(t);
            }
            foreach (var cc in Cc)
            {
                LogTrace(TraceLevel.Verbose, $"Adding {cc} to CC...");
                mailMessage.CC.Add(cc);
            }
            foreach (var bcc in Bcc)
            {
                LogTrace(TraceLevel.Verbose, $"Adding {bcc} to BCC...");
                mailMessage.Bcc.Add(bcc);
            }
            mailMessage.Body = message;
            mailMessage.Subject = Subject;
            LogTrace(TraceLevel.Info, $"Sending message to {To.Length + Cc.Length + Bcc.Length} total recipients...");
            client.Send(mailMessage);
            LogTrace(TraceLevel.Verbose, "Sent " + message);
            return 0;
        }

        private void LogTrace(TraceLevel level, string message)
        {
            if (!Trace.HasValue) return;
            if (Trace.level >= level)
            {
                Console.Out.WriteLine($"{level,7}: {message}");
            }
        }
    }

    public enum TraceLevel
    {
        Info = 0,
        Verbose,
    }
}
