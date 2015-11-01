﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using SAS.ClassSet.MemberInfo;
using SAS.ClassSet.Common;
using System.Web;
using System.Xml;
using Novacode;
namespace SAS.ClassSet.FunctionTools
{
    class WordTools
    {
        private void closefile()//删除word文档线程
        {
            try
            {
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("WINWORD"))
                {
                    p.Kill();
                }
            }
            catch (Exception)
            {
            }
        }
        //首席
        public string  Addchiefsupervisordata(WordTableInfo Info)
        {
            Common.Common.load_cheif_supervisor();
            object missingValue = System.Reflection.Missing.Value;
            object myTrue = false;                  //不显示Word窗口
            object fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";//WORD文档所在路径
            string newfile =  Common.Common.strAddfilesPath + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";//存储路径名称
            // object fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
            Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word._Document oDoc;
            oDoc = oWord.Documents.Open(ref fileName, ref missingValue,
               ref myTrue, ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue);
            Microsoft.Office.Interop.Word.Table newtable = oDoc.Tables[1];//获取word文档中的表格
            newtable.Cell(1, 2).Range.Text = Info.Teacher;
            newtable.Cell(1, 6).Range.Text = Info.Time.Substring(0, Info.Time.IndexOf(" "));
            newtable.Cell(2, 6).Range.Text = Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1);
            newtable.Cell(4, 2).Range.Text = Info.Class;
            newtable.Cell(5, 2).Range.Text = Info.Subject;
            object bSaveChange = true;
            oDoc.Close(ref bSaveChange, ref missingValue, ref missingValue);
            oDoc = null;
            oWord = null;

            closefile();
            if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
            {
                Directory.CreateDirectory(Common.Common.strAddfilesPath);
            }
          
            System.IO.File.Copy(fileName.ToString(), newfile, true);

            File.Delete(fileName.ToString());
            //sent_email(Supervisor, Time, Subject, newfile);
            //movetofile(newfile);
            return newfile; 
        }
        //一般
        public string fullcheifsupervisor(WordTableInfo Info)
        {   
            Common.Common.load_cheif_supervisor();
            string fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";//WORD文档所在路径
            string newfile =  Common.Common.strAddfilesPath + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";//存储路径名称
            DocX doc = DocX.Load(fileName);
            Table table = doc.Tables[0];
              
             table.Rows[0].Cells[1].Paragraphs[0].ReplaceText("teacher", Info.Teacher);
             table.Rows[0].Cells[5].Paragraphs[0].ReplaceText("time", Info.Time.Substring(0, Info.Time.IndexOf(" ")));
             table.Rows[1].Cells[5].Paragraphs[0].ReplaceText("address", Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1));
             table.Rows[3].Cells[1].Paragraphs[0].ReplaceText("class", Info.Class);
             table.Rows[4].Cells[1].Paragraphs[0].ReplaceText("context", Info.Subject);
             if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
             {
                 Directory.CreateDirectory(Common.Common.strAddfilesPath);
             }
             doc.SaveAs(newfile);

             doc.Dispose();
             return newfile;
        }
        public string fullsupervisor(WordTableInfo Info)
        {
            Common.Common.load_supervisor();
            string fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
            string newfile = Common.Common.strAddfilesPath + "\\" + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";
            DocX doc = DocX.Load(fileName1);
            doc.ReplaceText("title","广东医学院教师课堂教学质量评价表" + "(" + Info.Teachingtype + ")") ;
            Table table = doc.Tables[0];
            table.Rows[0].Cells[1].Paragraphs[0].ReplaceText("Teacher", Info.Teacher);
            table.Rows[0].Cells[3].Paragraphs[0].ReplaceText("Perfession", Info.Perfession);
            table.Rows[0].Cells[5].Paragraphs[0].ReplaceText("Time", Info.Time.Substring(0, Info.Time.IndexOf(" ")));
            table.Rows[1].Cells[1].Paragraphs[0].ReplaceText("Class", Info.Class);
            table.Rows[1].Cells[3].Paragraphs[0].ReplaceText("address", Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1));
            table.Rows[2].Cells[1].Paragraphs[0].ReplaceText("Context", Info.Subject);
            if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
            {
                Directory.CreateDirectory(Common.Common.strAddfilesPath);
            }
            doc.SaveAs(newfile);
            doc.Dispose();
            return newfile;
        }
        public void fullclasses(List<ExportClassInfo> info)
        {
            Common.Common.load_classes();
            string fileName1 = Environment.CurrentDirectory + "\\" + "classes.docx";
            string newfile = Common.Common.strAddfilesPath + "\\" + "东莞校区信息工程学院信工教师课程安排表（1.0)" + ".docx";
            DocX doc = DocX.Load(fileName1);
            MessageBox.Show("表格数量：" + doc.Tables.Count.ToString());
            for (int i = 0; i < info.Count; i++)
            {
                Table tb = doc.Tables[info[i].Week - 1];
                if (info[i].Start < 10)//上下午
                {
                    tb.Rows[info[i].Start].Cells[1 + info[i].Day].Paragraphs[0].ReplaceText(info[i].Start.ToString(), info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                    tb.Rows[info[i].End].Cells[1 + info[i].Day].Paragraphs[0].ReplaceText(info[i].End.ToString(), info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                    if (info[i].IsOverTop)
                    {
                        for (int k = info[i].Start + 1; k < info[i].End; k++)
                        {
                            tb.Rows[k].Cells[1 + info[i].Day].Paragraphs[0].ReplaceText((k).ToString(), info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                        }

                    }
                }
                else
                {
                    tb.Rows[info[i].Start].Cells[1 + info[i].Day].Paragraphs[0].ReplaceText(info[i].Start.ToString(), info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                    if (info[i].IsOverTop)
                    {
                        for (int k = info[i].Start + 1; k < info[i].End; k++)
                        {
                            tb.Rows[k].Cells[1 + info[i].Day].Paragraphs[0].ReplaceText((k).ToString(), info[i].Teachername + ":" + info[i].Classname + "(" + info[i].Classtype + ")");
                        }
                    }
                }
            }
            if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
            {
                Directory.CreateDirectory(Common.Common.strAddfilesPath);
            }
            doc.SaveAs(newfile);
            doc.Dispose();
        }
        public string  Addsupervisordata(WordTableInfo Info)
        {
            Common.Common.load_supervisor();
            object missingValue = System.Reflection.Missing.Value;
            object myTrue = false;                  //不显示Word窗口
            //object fileName = Environment.CurrentDirectory + "\\" + "chief_supervisor.doc";
            object fileName1 = Environment.CurrentDirectory + "\\" + "supervisor.doc";
            string newfile = Common.Common.strAddfilesPath+ "\\" + Info.Teacher + Info.Time.Trim() + Info.Supervisor + ".doc";
            Microsoft.Office.Interop.Word._Application oWord1 = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word._Document oDoc1;
            oDoc1 = oWord1.Documents.Open(ref fileName1, ref missingValue,
               ref myTrue, ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue, ref missingValue, ref missingValue,
               ref missingValue);
            Microsoft.Office.Interop.Word.Table newtable1 = oDoc1.Tables[1];
            oWord1.Selection.TypeText("广东医学院教师课堂教学质量评价表" + "(" + Info.Teachingtype + ")");
            newtable1.Cell(1, 2).Range.Text = Info.Teacher;
            newtable1.Cell(1, 4).Range.Text = Info.Perfession;
            newtable1.Cell(1, 6).Range.Text = Info.Time.Substring(0, Info.Time.IndexOf(" "));
            newtable1.Cell(2, 2).Range.Text = Info.Class;
            newtable1.Cell(2, 4).Range.Text = Info.Classroom + Info.Time.Substring(Info.Time.IndexOf(" ") + 1);
            newtable1.Cell(3, 2).Range.Text = Info.Subject;
            object bSaveChange = true;
            oDoc1.Close(ref bSaveChange, ref missingValue, ref missingValue);
            oDoc1 = null;
            oWord1 = null;



            closefile();
            if (!System.IO.File.Exists(Common.Common.strAddfilesPath))
            {
                Directory.CreateDirectory(Common.Common.strAddfilesPath);
            }
            System.IO.File.Copy(fileName1.ToString(), newfile, true);

            File.Delete(fileName1.ToString());
            return newfile;
           // sent_email(Supervisor, Time, Subject, newfile);
            // movetofile(newfile);
            //File.Move(newfile, cCommon.strAddfilesPath);
        }
    }
}
