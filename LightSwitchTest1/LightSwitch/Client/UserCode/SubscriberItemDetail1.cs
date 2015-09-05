using System;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.LightSwitch;
using Microsoft.LightSwitch.Framework.Client;
using Microsoft.LightSwitch.Presentation;
using Microsoft.LightSwitch.Presentation.Extensions;

namespace LightSwitchApplication
{
    public partial class SubscriberItemDetail1
    {
        partial void SubscriberItem_Loaded(bool succeeded)
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.SubscriberItem);
        }

        partial void SubscriberItem_Changed()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.SubscriberItem);
        }

        partial void SubscriberItemDetail1_Saved()
        {
            // Write your code here.
            this.SetDisplayNameFromEntity(this.SubscriberItem);
        }
    }
}