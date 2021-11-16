using System;
using System.Reflection;
using System.ServiceProcess;

namespace VisafeService
{
    public static class ServiceCommand
    {
        public const int SERVICE_CONTROL_PRESHUTDOWN = 0x0000000F;
        public const int SERVICE_ACCEPT_PRESHUTDOWN = 0x00000100;
        public const int SERVICE_CONTROL_SHUTDOWN = 0x00000005;
    }

    public partial class PreshutdownEnabledService : ServiceBase
    {
        public bool CanPreShutdown { get; private set; }

        public PreshutdownEnabledService()
        {
            FieldInfo acceptedCommandsField = typeof(ServiceBase).GetField("acceptedCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            if (acceptedCommandsField == null)
            {
                throw new InvalidOperationException("Field acceptedCommands not found in ServiceBase");
            }

            int acceptedCommands = (int)acceptedCommandsField.GetValue(this);
            acceptedCommands |= ServiceCommand.SERVICE_ACCEPT_PRESHUTDOWN;
            acceptedCommandsField.SetValue(this, acceptedCommands);
        }

        protected override void OnCustomCommand(int customCommand)
        {
            if (customCommand == ServiceCommand.SERVICE_CONTROL_PRESHUTDOWN)
            {
                var baseCallback = typeof(ServiceBase).GetMethod("ServiceCommandCallback", BindingFlags.Instance | BindingFlags.NonPublic);
                if (baseCallback == null)
                {
                    throw new InvalidOperationException("Method ServiceCommandCallback not found in ServiceBase");
                }

                try
                {
                    CanPreShutdown = true;
                    baseCallback.Invoke(this, new object[] { ServiceCommand.SERVICE_CONTROL_SHUTDOWN });
                }
                finally
                {
                    CanPreShutdown = false;
                }
            }
        }
    }
}
