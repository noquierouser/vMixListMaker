using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.UI.Notifications;

namespace vMixListMaker
{
    class ToastNotificationBuilder
    {
        public ToastNotification Build(string title, string content)
        {
            ToastVisual visual = new ToastVisual() {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },
                        new AdaptiveText()
                        {
                            Text = content
                        }
                    }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual
            };

            return new ToastNotification(toastContent.GetXml()) {
                ExpirationTime = DateTime.Now.AddSeconds(10)
            };
        }
    }
}
