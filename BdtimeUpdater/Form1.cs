using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using agi = HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace BdtimeUpdater
{
    public partial class Form1 : Form
    {
        // -----
        public int nTrNum = 5;
        string sWebSite = "http://lazytitan.dothome.co.kr/BdBossPhp/";
        public bool[] bCheck = new bool[4];

        // ----- HTML
        string[] sBosshtml = new string[4];     // 0 크자카,1 누배르,2 쿠툼,3 카란다

        // ----- Parsing
        agi.HtmlNode bossname;
        agi.HtmlNode bossdate;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
            {
                ResetBossTime();
                return;
            }
            var sFirst = "//*[@id='blackMain']//*[@id='powerbbsBody']/table/tr/td/div/table/tr/td/table/tr[3]/td/table/tbody/tr[";
            var sLast = "]/td[2]/a";

            var html = @"http://www.inven.co.kr/board/powerbbs.php?come_idx=4224";     // 웹 주소

            agi.HtmlWeb web = new agi.HtmlWeb();    // 걍 선언
            for (; ; nTrNum++)
            {
                if (nTrNum > 51)
                {
                    nTrNum = 5;
                    bCheck[0] = false;
                    bCheck[1] = false;
                    bCheck[2] = false;
                    bCheck[3] = false;
                }

                else
                {
                    var htmlDoc = web.Load(html);           // 웹로드

                    bossname = htmlDoc.DocumentNode.SelectSingleNode(sFirst + nTrNum + sLast);       // 테그 찾기     마지막 태그 td[4] = 날짜

                    bossdate = htmlDoc.DocumentNode.SelectSingleNode(sFirst + nTrNum + "]/td[4]");       //str.Contains

                    FindBossName();
                }
            }
        }

        void FindBossName()
        {
            if (bossname.OuterHtml.Contains("크자카") && !bCheck[0])
            {
                if (bossname.InnerText.IndexOf(':') == -1)
                {
                    Console.Write(bossdate.InnerText.Remove(5));
                    sBosshtml[0] = bossdate.InnerText.Remove(5);

                    HttpGet(sWebSite + "kzarka.php?kzarka='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[0] + "'");
                }
                else
                {
                    Console.WriteLine(CatchNum());
                    sBosshtml[0] = bossname.InnerText.Substring(bossname.InnerText.IndexOf(':') - 2, 5).Replace(';', '0');

                    HttpGet(sWebSite + "kzarka.php?kzarka='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[0] + "'");
                }
                bCheck[0] = true;
            }
            if (bossname.OuterHtml.Contains("누베르") && !bCheck[1])
            {
                if (bossname.InnerText.IndexOf(':') == -1)
                {
                    Console.Write(bossdate.InnerText.Remove(5));
                    sBosshtml[1] = bossdate.InnerText.Remove(5);

                    HttpGet(sWebSite + "nouver.php?nouver='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[1] + "'");
                }
                else
                {
                    Console.WriteLine(CatchNum());
                    sBosshtml[1] = bossname.InnerText.Substring(bossname.InnerText.IndexOf(':') - 2, 5).Replace(';', '0');

                    HttpGet(sWebSite + "nouver.php?nouver='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[1] + "'");
                }
                bCheck[1] = true;
            }
            if (bossname.OuterHtml.Contains("쿠툼") && !bCheck[2])
            {
                if (bossname.InnerText.IndexOf(':') == -1)
                {
                    sBosshtml[2] = bossdate.InnerText.Remove(5);

                    HttpGet(sWebSite + "kutum.php?kutum='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[2] + "'");
                }
                else
                {
                    Console.WriteLine(CatchNum());
                    sBosshtml[2] = bossname.InnerText.Substring(bossname.InnerText.IndexOf(':') - 2, 5).Replace(';', '0');

                    HttpGet(sWebSite + "kutum.php?kutum='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[2] + "'");
                }
                bCheck[2] = true;
            }
            if (bossname.OuterHtml.Contains("카란다") && !bCheck[3])
            {
                if (bossname.InnerText.IndexOf(':') == -1)
                {
                    sBosshtml[3] = bossdate.InnerText.Remove(5);

                    HttpGet(sWebSite + "karanda.php?karanda='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[3] + "'");
                }
                else
                {
                    sBosshtml[3] = bossname.InnerText.Substring(bossname.InnerText.IndexOf(':') - 2, 5).Replace(';', '0');

                    HttpGet(sWebSite + "karanda.php?karanda='" + DateTime.Now.ToString("yyyy-MM-dd") + " " + sBosshtml[3] + "'");
                }
                bCheck[3] = true;
            }
            label1.Text = bossname.OuterHtml + "\n\n";// + bossdate.OuterHtml;   // 테그 가져오기
            nTrNum++;
            return;
        }

        void ResetBossTime()
        {
            HttpGet(sWebSite + "Inspection.php?Reset='" + DateTime.Now.ToString("yyyy-MM-dd") + " 10:00:00'");
        }

        string CatchNum()
        {
            string sGetText;
            sGetText = Regex.Replace(bossname.InnerText, @"/D", "");        // 숫자만 제외하고 다 없에버리기
            return sGetText;
        }

        private void HttpGet(string urlStr)
        {
            WebClient webcli = new WebClient();

            webcli.DownloadString(urlStr);
        }
    }
}