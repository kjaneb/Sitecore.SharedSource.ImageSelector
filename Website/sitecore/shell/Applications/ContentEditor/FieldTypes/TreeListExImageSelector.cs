using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Web.UI.Sheer;
using Sitecore.Diagnostics;
using Sitecore.Configuration;
using Sitecore.Text;
using Sitecore.Globalization;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Resources;
using Sitecore.Web;
using System.IO;
using Sitecore.Data;
using System.Web.UI.WebControls;

namespace Sitecore.Shell.Applications.ContentEditor.FieldTypes
{
    public class TreeListExImageSelector : WebControl, IContentField, IMessageHandler
    {
        #region Properties

        public Database Database
        {
            get
            {
                UrlString str = new UrlString(this.Source);
                if (!string.IsNullOrEmpty(str["databasename"]))
                {
                    return Factory.GetDatabase(str["databasename"]);
                }
                return Sitecore.Context.ContentDatabase;
            }
        }

        public string ItemLanguage
        {
            get
            {
                return StringUtil.GetString(this.ViewState["ItemLanguage"]);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.ViewState["ItemLanguage"] = value;
            }
        }

        public string Source
        {
            get
            {
                return StringUtil.GetString(this.ViewState["Source"]);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.ViewState["Source"] = value;
            }
        }

        public string Value
        {
            get
            {
                return StringUtil.GetString(this.ViewState["Value"]);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.ViewState["Value"] = value;
            }
        }
        
        #endregion
        
        #region Methods
        protected void Edit(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (this.Enabled)
            {
                if (args.IsPostBack)
                {
                    if ((args.Result != null) && (args.Result != "undefined"))
                    {
                        string result = args.Result;
                        if (result == "-")
                        {
                            result = string.Empty;
                        }
                        if (this.Value != result)
                        {
                            Sitecore.Context.ClientPage.Modified = true;
                        }
                        this.Value = result;
                        HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
                        this.RenderItems(output);
                        SheerResponse.SetInnerHtml(this.ID, output.InnerWriter.ToString());
                    }
                }
                else
                {
                    UrlString urlString = new UrlString(UIUtil.GetUri("control:TreeListExImageSelector"));
                    
                    UrlHandle handle = new UrlHandle();
                    string str3 = this.Value;
                    if (str3 == "__#!$No value$!#__")
                    {
                        str3 = string.Empty;
                    }
                    handle["value"] = str3;
                    handle["source"] = this.Source;
                    handle["language"] = this.ItemLanguage;
                    handle.Add(urlString);
                    SheerResponse.ShowModalDialog(urlString.ToString(), "800px", "300px", string.Empty, true);
                    args.WaitForPostBack();
                }
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            output.Write("<div id=\"" + this.ID + "\" class=\"scContentControl\" style=\"height:125px;overflow:auto;padding:0px 4px\" ondblclick=\"javascript:return scForm.postEvent(this,event,'treelist:edit(id=" + this.ID + ")')\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
            this.RenderItems(output);
            output.Write("</div>");
        }

        private void RenderItems(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            foreach (string str in this.Value.Split(new char[] { '|' }))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Item item = this.Database.GetItem(str);
                    ImageBuilder builder = new ImageBuilder
                    {
                        Width = 0x80,
                        Height = 0x80,
                        Margin = "0px 4px 0px 0px",
                        Align = "left"
                    };
                    if (item == null)
                    {
                        builder.Src = "Applications/16x16/forbidden.png";
                        output.Write("<div>");
                        builder.Render(output);
                        output.Write(Translate.Text("Item not found: {0}", new object[] { str }));
                        output.Write("</div>");
                    }
                    else
                    {
                        builder.Src = item.Appearance.Icon;
                        output.Write("<div title=\"" + item.Paths.ContentPath + "\">");
                        builder.Render(output);
                        //output.Write(item.DisplayName);
                        output.Write("</div>");
                    }
                }
            }
        }
        #endregion

        #region Interfaces
        string IContentField.GetValue()
        {
            return this.Value;
        }

        void IContentField.SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            this.Value = value;
        }

        void IMessageHandler.HandleMessage(Message message)
        {
            string str;
            Assert.ArgumentNotNull(message, "message");
            if (((message["id"] == this.ID) && ((str = message.Name) != null)) && (str == "treelist:edit"))
            {
                Sitecore.Context.ClientPage.Start(this, "Edit");
            }
        }
        #endregion
    }
}