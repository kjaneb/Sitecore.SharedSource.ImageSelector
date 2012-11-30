using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Shell.Applications.ContentEditor.Dialogs.TreeListExImageSelector
{
    public class TreeListExImageSelectorForm : DialogForm
    {
        #region Fields
        protected XmlControl Dialog;
        protected TreeList TreeList;
        #endregion

        #region Methods
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                UrlHandle handle = UrlHandle.Get();
                this.TreeList.Source = handle["source"];
                this.TreeList.SetValue(StringUtil.GetString(new string[] { handle["value"] }));
                if (!string.IsNullOrEmpty(handle["title"]))
                {
                    this.Dialog["Header"] = handle["title"];
                }
                if (!string.IsNullOrEmpty(handle["text"]))
                {
                    this.Dialog["text"] = handle["text"];
                }
                if (!string.IsNullOrEmpty(handle["icon"]))
                {
                    this.Dialog["icon"] = handle["icon"];
                }
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            string str = this.TreeList.GetValue();
            if (str.Length == 0)
            {
                str = "-";
            }
            SheerResponse.SetDialogValue(str);
            base.OnOK(sender, args);
        }
        #endregion
    }
}