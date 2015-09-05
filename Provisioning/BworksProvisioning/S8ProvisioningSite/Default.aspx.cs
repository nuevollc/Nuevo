using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using BworksProvisioning.net.strata8.lab.ews1;
using Strata8.Telephony.Provisioning.Services;

public partial class _Default : System.Web.UI.Page 
{
    private Strata8.Telephony.Provisioning.Services.BworksProvisioner bp;

    protected void Page_Load(object sender, EventArgs e)
    {
        bp = new BworksProvisioner();
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        bp = new BworksProvisioner();
        bp.ProcessLogIn();

    }
}
