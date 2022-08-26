using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace YoutubeLiveChat
{
    public partial class Form1 : Form
    {
        String webPath;
        String workingPath;
        CefSettings settings;
        ChromiumWebBrowser browser;

        void initChrome()
        {
            settings = new CefSettings();
            settings.LogFile = workingPath + "debug.log";
            settings.CefCommandLineArgs.Add("persistant_sessioin_cookie", "1");
            settings.CachePath = workingPath;
            Cef.Initialize(settings, performDependencyCheck: true);
        }

        public Form1()
        {
            InitializeComponent();
            workingPath = Environment.ExpandEnvironmentVariables("%userprofile%") + "\\documents\\s2cache\\";
            initChrome();
            Console.WriteLine("1");
        }

        public void processNewLine(String name, String text)
        {
            Console.WriteLine(name + " >>> " + text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            webPath = null;
            panel1.Dock = DockStyle.Fill;
            browser = new ChromiumWebBrowser(textBox1.Text);
            browser.BrowserSettings = new BrowserSettings();
            browser.JavascriptObjectRepository.Register("bound", new cefCustomObject(this), false, options: BindingOptions.DefaultBinder);
            browser.FrameLoadEnd += (object o, FrameLoadEndEventArgs fleea) =>
            {
                Console.WriteLine("aa");

                if (webPath == null && fleea.Frame.Url.Split('=')[0] == "https://www.youtube.com/live_chat?continuation")
                {
                    Console.WriteLine("a");
                    browser.Load(webPath = fleea.Frame.Url);
                }
                else if(webPath == fleea.Frame.Url)
                {
                    Console.WriteLine("b");
                    browser.ShowDevTools();
                    fleea.Frame.ExecuteJavaScriptAsync("(async function(){ await CefSharp.BindObjectAsync('boundAsync', 'bound');   var last = ''; setInterval(function(){ (function(l){ if(last!=(cid = l[l.length-1].id))for(var i = l.length; i--;) { if(last==l[i].id)return (last=cid); if(l[i].children[1].children[2].textContent)bound.onText(l[i].children[1].children[1].children[0].textContent, l[i].children[1].children[2].textContent); }; last = cid; })(document.getElementsByTagName('yt-live-chat-text-message-renderer')); }, 1000);     })()");
                }
            };
            browser.Dock = DockStyle.Fill;
            panel1.Controls.Add(browser);

        }

        class cefCustomObject
        {
            private static Form1 mainForm;
            public cefCustomObject(Form1 form)
            {
                mainForm = form;
            }

            public void onText(String name, String text)
            {
                mainForm.processNewLine(name, text);
            }
        }
    }
}
