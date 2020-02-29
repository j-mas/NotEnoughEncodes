﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace NotEnoughEncodes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Get Number of Cores
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            //Sets the Number of Workers = Phsyical Core Count
            int tempCorecount = coreCount * 1 / 2;
            TextBoxNumberWorkers.Text = tempCorecount.ToString();

            //Load Settings
            readSettings();
        }

        public void readSettings()
        {
            try
            {
                //If settings.ini exist -> Set all Values
                bool fileExist = File.Exists("settings.ini");

                if (fileExist)
                {
                    string[] lines = System.IO.File.ReadAllLines("settings.ini");

                    TextBoxNumberWorkers.Text = lines[0];
                    ComboBoxCpuUsed.Text = lines[1];
                    ComboBoxBitdepth.Text = lines[2];
                    TextBoxEncThreads.Text = lines[3];
                    TextBoxcqLevel.Text = lines[4];
                    TextBoxKeyframeInterval.Text = lines[5];
                    TextBoxTileCols.Text = lines[6];
                    TextBoxTileRows.Text = lines[7];
                    ComboBoxPasses.Text = lines[8];
                    TextBoxFramerate.Text = lines[9];
                    ComboBoxEncMode.Text = lines[10];
                    TextBoxChunkLength.Text = lines[11];
                }

            }
            catch { }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Saves all Current Settings to a file
            string maxConcurrency = TextBoxNumberWorkers.Text;
            string cpuUsed = ComboBoxCpuUsed.Text;
            string bitDepth = ComboBoxBitdepth.Text;
            string encThreads = TextBoxEncThreads.Text;
            string cqLevel = TextBoxcqLevel.Text;
            string kfmaxdist = TextBoxKeyframeInterval.Text;
            string tilecols = TextBoxTileCols.Text;
            string tilerows = TextBoxTileRows.Text;
            string nrPasses = ComboBoxPasses.Text;
            string fps = TextBoxFramerate.Text;
            string encMode = this.ComboBoxEncMode.Text;
            string chunkLength = TextBoxChunkLength.Text;

            string[] lines = { maxConcurrency, cpuUsed, bitDepth, encThreads, cqLevel, kfmaxdist, tilecols, tilerows, nrPasses, fps, encMode, chunkLength };
            System.IO.File.WriteAllLines("settings.ini", lines);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                textBlockPath.Text = openFileDialog.FileName;
        }

        private void ButtonOutput_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Matroska|*.mkv";
            if (saveFileDialog.ShowDialog() == true)
                textBlockOutput.Text = saveFileDialog.FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Start MainClass
            MainClass();
        }

        public static class Cancel
        {
            public static Boolean CancelAll = false;

        }



        public void MainClass()
        {
            //Sets Label
            pLabel.Dispatcher.Invoke(() => pLabel.Content = "Starting...", DispatcherPriority.Background);

            //Sets the working directory
            string currentPath = Directory.GetCurrentDirectory();
            //Checks if Chunks folder exist, if no it creates Chunks folder
            if (!Directory.Exists(Path.Combine(currentPath, "Chunks")))
                Directory.CreateDirectory(Path.Combine(currentPath, "Chunks"));

            //Start Splitting
            String videoInput = textBlockPath.Text;
            string videoOutput = textBlockOutput.Text;

            if (CheckBoxResume.IsChecked == false)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                //FFmpeg Arguments

                //Checks if Source needs to be reencoded
                if (CheckBoxReencode.IsChecked == false)
                {
                    startInfo.Arguments = "/C ffmpeg.exe -i " + '\u0022' + videoInput + '\u0022' + " -vcodec copy -f segment -segment_time " + TextBoxChunkLength.Text + " -an " + '\u0022' + "Chunks\\out%0d.mkv" + '\u0022';

                }
                else if (CheckBoxReencode.IsChecked == true)
                {
                    startInfo.Arguments = "/C ffmpeg.exe -i " + '\u0022' + videoInput + '\u0022' + " -c:v utvideo -f segment -segment_time " + TextBoxChunkLength.Text + " -an " + '\u0022' + "Chunks\\out%0d.mkv" + '\u0022';

                }
                //Console.WriteLine(startInfo.Arguments);
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

            }

            //Create Array List with all Chunks
            string[] chunks;
            //Sets the Chunks directory
            String sdira = currentPath + "\\Chunks";
            //Add all Files in Chunks Folder to array
            chunks = Directory.GetFiles(sdira, "*mkv", SearchOption.AllDirectories).Select(x => Path.GetFileName(x)).ToArray();


            if (CheckBoxResume.IsChecked == false)
            {
                //Checks if there is more than 9 Chunks, inorder to rename them, because later in order to concat, it has to be sortet (Values from xx-xx instead of x-xx)
                if (chunks.Count() >= 10)
                {
                                       
                    System.IO.File.Move(sdira + "\\out0.mkv", sdira + "\\out00.mkv");
                    System.IO.File.Move(sdira + "\\out1.mkv", sdira + "\\out01.mkv");
                    System.IO.File.Move(sdira + "\\out2.mkv", sdira + "\\out02.mkv");
                    System.IO.File.Move(sdira + "\\out3.mkv", sdira + "\\out03.mkv");
                    System.IO.File.Move(sdira + "\\out4.mkv", sdira + "\\out04.mkv");
                    System.IO.File.Move(sdira + "\\out5.mkv", sdira + "\\out05.mkv");
                    System.IO.File.Move(sdira + "\\out6.mkv", sdira + "\\out06.mkv");
                    System.IO.File.Move(sdira + "\\out7.mkv", sdira + "\\out07.mkv");
                    System.IO.File.Move(sdira + "\\out8.mkv", sdira + "\\out08.mkv");
                    System.IO.File.Move(sdira + "\\out9.mkv", sdira + "\\out09.mkv");
                    
                    //Clears the Array
                    //Array.Clear(chunks, 0, chunks.Length);
                    //Reads all Files to array
                    //chunks = Directory.GetFiles(sdira, "*.mkv", SearchOption.AllDirectories).Select(x => Path.GetFileName(x)).ToArray();
                }

            }


            //Parse Textbox Text to String for loop threading
            int maxConcurrency = Int16.Parse(TextBoxNumberWorkers.Text);
            int cpuUsed = Int16.Parse(ComboBoxCpuUsed.Text);
            int bitDepth = Int16.Parse(ComboBoxBitdepth.Text);
            int encThreads = Int16.Parse(TextBoxEncThreads.Text);
            int cqLevel = Int16.Parse(TextBoxcqLevel.Text);
            int kfmaxdist = Int16.Parse(TextBoxKeyframeInterval.Text);
            int tilecols = Int16.Parse(TextBoxTileCols.Text);
            int tilerows = Int16.Parse(TextBoxTileRows.Text);
            //int nrPasses = Int16.Parse(TextBoxPasses.Text);
            int nrPasses = Int16.Parse(ComboBoxPasses.Text);
            string fps = TextBoxFramerate.Text;
            string encMode = this.ComboBoxEncMode.Text;
            Boolean resume = false;

            //Sets Resume Mode
            if (CheckBoxResume.IsChecked == true)
            {
                resume = true;

                foreach (string line in File.ReadLines("encoded.txt"))
                {
                    //Removes all Items from Arraylist which are in encoded.txt
                    chunks = chunks.Where(s => s != line).ToArray();

                }
                //Set the Maximum Value of Progressbar
                prgbar.Maximum = chunks.Count();
            }
            
            //Starts the async task
            StartTask(maxConcurrency, cpuUsed, bitDepth, encThreads, cqLevel, kfmaxdist, tilerows, tilecols, nrPasses, fps, encMode, resume, videoOutput);
            //Set Maximum of Progressbar
            prgbar.Maximum = chunks.Count();
            //Set the Progresslabel to 0 out of Number of chunks, because people would think that it doesnt to anything
            pLabel.Dispatcher.Invoke(() => pLabel.Content = "0 / " + prgbar.Maximum, DispatcherPriority.Background);

        }

        //Async Class -> UI doesnt freeze
        private async void StartTask(int maxConcurrency, int cpuUsed, int bitDepth, int encThreads, int cqLevel, int kfmaxdist, int tilerows, int tilecols, int passes, string fps, string encMode, Boolean resume, string videoOutput)
        {
            //Run encode class async
            await Task.Run(() => encode(maxConcurrency, cpuUsed, bitDepth, encThreads, cqLevel, kfmaxdist, tilerows, tilecols, passes, fps, encMode, resume, videoOutput));
        }

        //Main Encoding Class
        public void encode(int maxConcurrency, int cpuUsed, int bitDepth, int encThreads, int cqLevel, int kfmaxdist, int tilerows, int tilecols, int passes, string fps, string encMode, Boolean resume, string videoOutput)
        {
            //Set Working directory
            string currentPath = Directory.GetCurrentDirectory();

 
            
            //Create Array List with all Chunks
            string[] chunks;
            //Sets the Chunks directory
            string sdira = currentPath + "\\Chunks";

            //Add all Files in Chunks Folder to array
            chunks = Directory.GetFiles(sdira, "*mkv", SearchOption.AllDirectories).Select(x => Path.GetFileName(x)).ToArray();

            if (resume == true)
            {
                //Removes all Items from Arraylist which are in encoded.txt
                foreach (string line in File.ReadLines("encoded.txt"))
                {
                    chunks = chunks.Where(s => s != line).ToArray();

                }

            }


            //Get Number of chunks for label of progressbar
            string labelstring = chunks.Count().ToString();

            string finalEncodeMode = "";

            //Sets the Encoding Mode
            if (encMode == "q")
            {
                finalEncodeMode = " --end-usage=q --cq-level="+ cqLevel;

            }else if (encMode == "vbr")
            {
                //If vbr set finalEncodeMode
                finalEncodeMode = " --end-usage=vbr --target-bitrate=" + cqLevel;
            }else if (encMode == "cbr")
            {
                //If cbr set finalEncodeMode
                finalEncodeMode = " --end-usage=cbr --target-bitrate=" + cqLevel;
            }

            
            
                //Parallel Encoding - aka some blackmagic
                using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(maxConcurrency))
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var items in chunks)
                    {
                        concurrencySemaphore.Wait();
                      
                        var t = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                if (Cancel.CancelAll == false)
                                {
                                    if (passes == 1)
                                    {
                                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                        startInfo.FileName = "cmd.exe";
                                        startInfo.Arguments = "/C ffmpeg.exe -i " + '\u0022' + sdira + "\\" + items + '\u0022' + " -pix_fmt yuv420p -vsync 0 -f yuv4mpegpipe - | aomenc.exe - --passes=1 --cpu-used=" + cpuUsed + " --threads=" + encThreads + finalEncodeMode + " --bit-depth=" + bitDepth + " --tile-columns=" + tilecols + " --fps=" + fps + " --tile-rows=" + tilerows + " --kf-max-dist=" + kfmaxdist + " --output=Chunks\\" + items + "-av1.ivf";
                                        process.StartInfo = startInfo;
                                        //Console.WriteLine(startInfo.Arguments);
                                        process.Start();
                                        process.WaitForExit();
                                        //Progressbar +1
                                        prgbar.Dispatcher.Invoke(() => prgbar.Value += 1, DispatcherPriority.Background);
                                        //Label of Progressbar = Progressbar
                                        pLabel.Dispatcher.Invoke(() => pLabel.Content = prgbar.Value + " / " + labelstring, DispatcherPriority.Background);
                                        if (Cancel.CancelAll == false)
                                        {
                                            //Write Item to file for later resume if something bad happens
                                            WriteToFileThreadSafe(items, "encoded.txt");
                                         
                                        }
                                        else
                                        {
                                            KillInstances();
                                        }


                                    }
                                    else if (passes == 2)
                                    {
                                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                        startInfo.FileName = "cmd.exe";
                                        startInfo.Arguments = "/C ffmpeg.exe -i " + '\u0022' + sdira + "\\" + items + '\u0022' + " -pix_fmt yuv420p -vsync 0 -f yuv4mpegpipe - | aomenc.exe - --passes=2 --pass=1 --fpf=Chunks\\" + items + "_stats.log --cpu-used=" + cpuUsed + " --threads=" + encThreads + finalEncodeMode + " --bit-depth=" + bitDepth + " --fps=" + fps + " --tile-columns=" + tilecols + " --tile-rows=" + tilerows + " --kf-max-dist=" + kfmaxdist + " --output=NUL";
                                        process.StartInfo = startInfo;
                                        //Console.WriteLine(startInfo.Arguments);
                                        process.Start();
                                        process.WaitForExit();

                                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                        startInfo.FileName = "cmd.exe";
                                        startInfo.Arguments = "/C ffmpeg.exe -i " + '\u0022' + sdira + "\\" + items + '\u0022' + " -pix_fmt yuv420p -vsync 0 -f yuv4mpegpipe - | aomenc.exe - --passes=2 --pass=2 --fpf=Chunks\\" + items + "_stats.log --cpu-used=" + cpuUsed + " --threads=" + encThreads + finalEncodeMode + " --bit-depth=" + bitDepth + " --fps=" + fps + " --tile-columns=" + tilecols + " --tile-rows=" + tilerows + " --kf-max-dist=" + kfmaxdist + " --output=Chunks\\" + items + "-av1.ivf";
                                        process.StartInfo = startInfo;
                                        //Console.WriteLine(startInfo.Arguments);
                                        process.Start();
                                        process.WaitForExit();
                                        prgbar.Dispatcher.Invoke(() => prgbar.Value += 1, DispatcherPriority.Background);
                                        pLabel.Dispatcher.Invoke(() => pLabel.Content = prgbar.Value + " / " + labelstring, DispatcherPriority.Background);
                                        if (Cancel.CancelAll == false)
                                        {
                                            //Write Item to file for later resume if something bad happens
                                            WriteToFileThreadSafe(items, "encoded.txt");

                                        }
                                        else
                                        {
                                            KillInstances();
                                        }
                                    }
                                

                                }

                            }
                            finally
                            {
                                concurrencySemaphore.Release();
                            }
                        });

                        tasks.Add(t);

                    }

                    Task.WaitAll(tasks.ToArray());

                }

                //Mux all Encoded chunks back together
                concat(videoOutput);

        }

        //Mux ivf Files back together
        private void concat(string videoOutput)
        {
            if (Cancel.CancelAll == false)
            {
                string currentPath = Directory.GetCurrentDirectory();

                string outputfilename = videoOutput;

                pLabel.Dispatcher.Invoke(() => pLabel.Content = "Muxing files", DispatcherPriority.Background);

                //Lists all ivf files in mylist.txt
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                //FFmpeg Arguments
                startInfo.Arguments = "/C (for %i in (Chunks\\*.ivf) do @echo file '%i') > Chunks\\mylist.txt";
                //Console.WriteLine(startInfo.Arguments);
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                //Concat the Videos
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                //FFmpeg Arguments
                startInfo.Arguments = "/C ffmpeg.exe -f concat -safe 0 -i Chunks\\mylist.txt -c copy "+ '\u0022' + outputfilename + '\u0022';
                //Console.WriteLine(startInfo.Arguments);
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                pLabel.Dispatcher.Invoke(() => pLabel.Content = "Muxing completed!", DispatcherPriority.Background);

            }

        }

        //Kill all aomenc instances
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            Cancel.CancelAll = true;
            KillInstances();
        }

        public void KillInstances()
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("aomenc"))
                {
                    process.Kill();
                }
                foreach (var process in Process.GetProcessesByName("ffmpeg"))
                {
                    process.Kill();
                }

            }
            catch { }


        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                //Delete Files, because of lazy dump****
                File.Delete("encoded.txt");
                Directory.Delete("Chunks", true);

            }
            catch { }
        }

        //Some smaller Blackmagic, so parallel Workers won't lockdown the encoded.txt file
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        public void WriteToFileThreadSafe(string text, string path)
        {
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                // Append text to the file
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }

    }
}
