using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero;

namespace WRFNCProcessor
{
    class Program
    {
        public static DateTime forecasteDateManual = new DateTime();
        static void Main(string[] args)
        {
            if (!DHI.Mike.Install.MikeImport.Setup(18, DHI.Mike.Install.MikeProducts.MikeCore))
                throw new Exception("Cannot find a proper MIKE installation");

            try
            {
                int bytesRead = 0;
                byte[] buffer = new byte[2048];

                //FtpWebRequest request = CreateFtpWebRequest("ftp://192.168.3.7/wrfout_d01", "ftpuser", "ffwc*iwm", true); //"ftpuser", "ffwc*iwm" "fmguser", "fmg*user" ----for BWDB
                //FtpWebRequest request = CreateFtpWebRequest("ftp://182.160.115.59/wrfout_d01", "fmguser", "fmg*iwm66", true); //"ftp://192.168.3.7/wrfout_d01", "ffiwm", "ffiwm123!@#", true ---old one
            FtpWebRequest request = CreateFtpWebRequest("ftp://182.160.115.59/wrfout_d01", "ftpguest", "iwmffws@fmg", true); //"ftp://192.168.3.7/wrfout_d01", "ffiwm", "ffiwm123!@#", true ---old one
            request.Method = WebRequestMethods.Ftp.DownloadFile;
                //request.Method = WebRequestMethods.Ftp.DownloadFile;

                Stream reader = request.GetResponse().GetResponseStream();
                FileStream fileStream = new FileStream(@"E:\FFWS\Model\WRF\wrfout_d01", FileMode.Create);

                while (true)
                {
                    bytesRead = reader.Read(buffer, 0, buffer.Length);
                    Console.Write("*");

                    if (bytesRead == 0)
                        break;

                    fileStream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
                Console.WriteLine("\n WRF Result File Copy Is Completed");


            string wrfpath = null;
            DirectoryInfo di = new DirectoryInfo(@"E:\FFWS\Model\WRF");

            foreach (FileInfo fi in di.GetFiles("*."))
            {
                wrfpath = fi.FullName;
            }
            File.Copy(wrfpath, wrfpath + ".nc", true);
            File.Delete(wrfpath);
            Console.WriteLine("WRF netCDF file processing started");
            //var dataset = Microsoft.Research.Science.Data.DataSet.Open(wrfpath + ".nc" + "?openMode=readOnly");
            var dataset = Microsoft.Research.Science.Data.DataSet.Open(@"E:\FFWS\Model\WRF\" + "wrfout_d01.nc" + "?openMode=readOnly");
            float[,,] Xlong = dataset.GetData<float[,,]>("XLONG");
            float[,,] Xlat = dataset.GetData<float[,,]>("XLAT");
            float[,,] rain = dataset.GetData<float[,,]>("RAINC");
            float[,,] rainnc = dataset.GetData<float[,,]>("RAINNC");

            string startDateText = dataset.GetAttr(0, "START_DATE").ToString();
            StringBuilder sb = new StringBuilder();
            DateTime startDate = DateTime.Parse(startDateText.Substring(0, 10) + " " + startDateText.Substring(11, 8)).AddHours(6);
            sb.AppendLine(startDate.ToString("yyyy-MM-dd"));
            for (int i = 0; i < 7; i++)
            {
                sb.AppendLine(startDate.AddDays(i).ToString("yyyy-MM-dd hh:mm tt"));
            }
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\DateTime.txt", sb.ToString());
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            StringBuilder sb4 = new StringBuilder();
            StringBuilder sb5 = new StringBuilder();
            StringBuilder sb0 = new StringBuilder();

            sb0.AppendLine("Point");
            sb1.AppendLine("Point");
            sb2.AppendLine("Point");
            sb3.AppendLine("Point");
            sb4.AppendLine("Point");
            sb5.AppendLine("Point");
            int count = 0;
            for (int i = 0; i < 159; i++)
            {
                for (int j = 0; j < 159; j++)
                {
                    sb0.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + (rain[4, i, j] + rainnc[4, i, j]).ToString("0.00") + " " + (rain[4, i, j] + rainnc[4, i, j]).ToString("0.00"));
                    sb1.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + ((rain[8, i, j] + rainnc[8, i, j]) - (rain[4, i, j] + rainnc[4, i, j])).ToString("0.00") + " " + ((rain[8, i, j] + rainnc[8, i, j]) - (rain[4, i, j] + rainnc[4, i, j])).ToString("0.00"));
                    sb2.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + ((rain[12, i, j] + rainnc[12, i, j]) - (rain[8, i, j] + rainnc[8, i, j])).ToString("0.00") + " " + ((rain[12, i, j] + rainnc[12, i, j]) - (rain[8, i, j] + rainnc[8, i, j])).ToString("0.00"));
                    sb3.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + ((rain[16, i, j] + rainnc[16, i, j]) - (rain[12, i, j] + rainnc[12, i, j])).ToString("0.00") + " " + ((rain[16, i, j] + rainnc[16, i, j]) - (rain[12, i, j] + rainnc[12, i, j])).ToString("0.00"));
                    sb4.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + ((rain[20, i, j] + rainnc[20, i, j]) - (rain[16, i, j] + rainnc[16, i, j])).ToString("0.00") + " " + ((rain[20, i, j] + rainnc[20, i, j]) - (rain[16, i, j] + rainnc[16, i, j])).ToString("0.00"));
                    sb5.AppendLine(count + " " + Xlong[0, 0, j] + " " + Xlat[0, i, 0] + " " + ((rain[24, i, j] + rainnc[24, i, j]) - (rain[20, i, j] + rainnc[20, i, j])).ToString("0.00") + " " + ((rain[24, i, j] + rainnc[24, i, j]) - (rain[20, i, j] + rainnc[20, i, j])).ToString("0.00"));
                    count = count + 1;
                }
            }
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF0.txt", sb0.ToString());
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF1.txt", sb1.ToString());
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF2.txt", sb2.ToString());
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF3.txt", sb3.ToString());
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF4.txt", sb4.ToString());
            File.WriteAllText(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF5.txt", sb5.ToString());
            //Process.Start(@"E:\Rain_MAP\WRF\Scripts\Rain_MAP_All.py");

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Python27\ArcGIS10.8\python.exe";
            startInfo.Arguments = @"E:\FFWS\GIS_DataBase\WRF_MAP\Rain_MAP_All.py";
            process.StartInfo = startInfo;
            process.Start();

            foreach (FileInfo fi in di.GetFiles("*."))
                {
                    File.Delete(fi.FullName);
                }
        }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            //Console.ReadKey();


            #region WRF dfs0 preparation.......

            try
            {
                var ForecastDay_FF_Model = DateTime.Today;
                forecasteDateManual = new DateTime(ForecastDay_FF_Model.Year, ForecastDay_FF_Model.Month, ForecastDay_FF_Model.Day, 0, 0, 0);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("WRF-DFS0 Data processing started....");
                float[,,] gridR = new float[6, 159, 159];
                var dataset = Microsoft.Research.Science.Data.DataSet.Open(@"E:\FFWS\Model\WRF\wrfout_d01.nc?openMode=readOnly");
                float[,,] Xlong = dataset.GetData<float[,,]>("XLONG");
                float[,,] Xlat = dataset.GetData<float[,,]>("XLAT");

                float[,,] rain = dataset.GetData<float[,,]>("RAINC");
                float[,,] rainnc = dataset.GetData<float[,,]>("RAINNC");
                string startDateText = dataset.GetAttr(0, "START_DATE").ToString();

                for (int k = 0; k < 6; k++)
                {
                    for (int i = 0; i < 159; i++)
                    {
                        for (int j = 0; j < 159; j++)
                        {
                            gridR[k, i, j] = rain[(k + 1) * 4, i, j] + rainnc[(k + 1) * 4, i, j];
                        }
                    }
                }

                string[] gsMapPointInfo = File.ReadAllLines(@"E:\FFWS\DataBase\SupportingFile\GBM_WRF.txt");
                string[] catchment = new string[gsMapPointInfo.Length];
                int[] gridI = new int[gsMapPointInfo.Length];
                int[] gridJ = new int[gsMapPointInfo.Length];

                for (int i = 0; i < gsMapPointInfo.Length; i++)
                {
                    var parsed = gsMapPointInfo[i].Split(',');
                    catchment[i] = parsed[0];
                    gridI[i] = int.Parse(parsed[1]);
                    gridJ[i] = int.Parse(parsed[2]);
                }
                string[] catchmentName = catchment.Distinct().ToArray();
                StringBuilder sb = new StringBuilder();

                foreach (string element in catchmentName)
                {
                    //DateTime today = DateTime.Parse(startDateText.Substring(0, 10) + " " + startDateText.Substring(11, 8)).AddHours(6); // WRF date using
                    DateTime today = forecasteDateManual.AddHours(6); /// forecast using
                    float[] catchrain = { -1e-25f, 0, 0, 0, 0, 0, 0 };
                    int count = 0;
                    for (int i = 0; i < catchment.Length; i++)
                    {
                        if (element == catchment[i])
                        {
                            count = count + 1;
                            catchrain[1] = catchrain[1] + gridR[0, gridI[i], gridJ[i]];
                            catchrain[2] = catchrain[2] + gridR[1, gridI[i], gridJ[i]] - gridR[0, gridI[i], gridJ[i]];
                            catchrain[3] = catchrain[3] + gridR[2, gridI[i], gridJ[i]] - gridR[1, gridI[i], gridJ[i]];
                            catchrain[4] = catchrain[4] + gridR[3, gridI[i], gridJ[i]] - gridR[2, gridI[i], gridJ[i]];
                            catchrain[5] = catchrain[5] + gridR[4, gridI[i], gridJ[i]] - gridR[3, gridI[i], gridJ[i]];
                            catchrain[6] = catchrain[6] + gridR[5, gridI[i], gridJ[i]] - gridR[4, gridI[i], gridJ[i]];
                        }
                    }
                    for (int i = 1; i < 7; i++)
                    {
                        catchrain[i] = catchrain[i] / count;
                    }

                    DateTime dfsDate = today;
                    DfsFactory factory = new DfsFactory();
                    string filename = @"E:\FFWS\Model\NAM\WRF-DFS0\" + element + ".dfs0";
                    DfsBuilder filecreator = DfsBuilder.Create(element, element, 2012);
                    filecreator.SetDataType(1);
                    filecreator.SetGeographicalProjection(factory.CreateProjectionUndefined());

                    filecreator.SetTemporalAxis(factory.CreateTemporalNonEqCalendarAxis(eumUnit.eumUsec, new DateTime(dfsDate.Year, dfsDate.Month, dfsDate.Day, dfsDate.Hour, dfsDate.Minute, dfsDate.Second)));
                    filecreator.SetItemStatisticsType(StatType.RegularStat);
                    DfsDynamicItemBuilder item = filecreator.CreateDynamicItemBuilder();
                    item.Set(element, eumQuantity.Create(eumItem.eumIRainfall, eumUnit.eumUmillimeter), DfsSimpleType.Float);
                    item.SetValueType(DataValueType.StepAccumulated);
                    item.SetAxis(factory.CreateAxisEqD0());
                    item.SetReferenceCoordinates(1f, 2f, 3f);
                    filecreator.AddDynamicItem(item.GetDynamicItemInfo());

                    filecreator.CreateFile(filename);
                    IDfsFile file = filecreator.GetFile();
                    IDfsFileInfo fileinfo = file.FileInfo;
                    fileinfo.DeleteValueFloat = -1e-25f;

                    for (int i = 0; i < 7; i++)
                    {
                        file.WriteItemTimeStepNext((today - dfsDate).TotalSeconds, new float[] { catchrain[i] });
                        today = today.AddDays(1);
                    }
                    file.Close();
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(catchmentName.Length + " station's WRF Rainfall data created successfully.");
            }
            catch (Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WRF DFS0 cannot created for an error. Error: " + error.Message);
                Console.ReadKey();
                Environment.Exit(1);
            }

            #endregion

            //try
            //{

            //    MailMessage mail = new MailMessage("NoReply@iwmbd.org", "rna@iwmbd.org");
            //    SmtpClient client = new SmtpClient();
            //    client.Port = 25;
            //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    client.UseDefaultCredentials = false;
            //    client.Host = "10.0.0.2";
            //    mail.Subject = "WRF_Model Output";
            //    mail.Body = "Md. Shahadat Hossain \nJunior Specialist \nFlood Management Division \nInstitute of Water Modelling \nMohakhali DOHS, Dhaka-1206\nBangladesh.";
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\DateTime.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF0.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF1.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF2.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF3.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF4.txt"));
            //    mail.Attachments.Add(new Attachment(@"E:\FFWS\GIS_DataBase\WRF_MAP\Shape_Raster\RF5.txt"));

            //    client.Send(mail);
            //    Console.WriteLine("Mail is sent");

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Unable to send email. Error : " + ex);
            //}
            //Console.ReadKey();
        }
        private static FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, string userName, string password, bool keepAlive = false)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpDirectoryPath));

            //Set proxy to null. Under current configuration if this option is not set then the proxy that is used will get an html response from the web content gateway (firewall monitoring system)
            request.Proxy = null;

            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = keepAlive;

            request.Credentials = new NetworkCredential(userName, password);

            return request;
        }
    }
}
