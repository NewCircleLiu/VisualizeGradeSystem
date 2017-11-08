using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

using NPOI.SS.UserModel;
using VisualizeGradeSystem.Models.Score;
using VisualizeGradeSystem.Models.Puzzle;
using VisualizeGradeSystem.Models.Knowledge;

using System.IO;

namespace VisualizeGradeSystem.Utils
{
    //把Excel里面的指定行读入数据库
    public class ReadExcel
    {
        public Knowledge[] readKowledge(string filepath, int len,Puzzle [] pList)
        {
            IWorkbook wk = null;
            List<Knowledge> list = new List<Knowledge>();
            string extension = Path.GetExtension(filepath);
            FileStream fs = File.OpenRead(filepath);
            if (extension.Equals(".xls"))
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            else
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            ISheet sheet = wk.GetSheetAt(7);
            IRow row = null;  //读取当前行数据
            fs.Close();
            for (int i = 1; i < sheet.LastRowNum; i=i+2)
            {
                row = sheet.GetRow(i);  //读取当前行数据
                if (row != null)
                {
                    for (int j = 0; j < len; j++)
                    {
                        Knowledge k = new Knowledge();
                        k.score_gradeid = GetCellValue(row.GetCell(0)).ToString();
                        k.stu_name = GetCellValue(row.GetCell(1)).ToString();
                        k.stu_id = GetCellValue(row.GetCell(2)).ToString();
                        k.puzzle_name = pList[j].puzzle_name;
                        k.knowledge_name = pList[j].puzzle_knowledge;
                        double s = Convert.ToDouble(GetCellValue(sheet.GetRow(i + 1).GetCell(3 + j)).ToString());
                        k.score = 100 * (s / pList[j].puzzle_fullscore);
                        k.uploadtime = DateTime.Now.ToString("yyyy-MM-dd");
                        k.subject = pList[j].subject;
                        list.Add(k);
                    }
                }
            }
            return list.ToArray();  
        }
        public Puzzle[] readPuzzle(string filepath)
        {
            IWorkbook wk = null;
            List<Puzzle> list = new List<Puzzle>();
            string extension = Path.GetExtension(filepath);
            FileStream fs = File.OpenRead(filepath);
            if (extension.Equals(".xls"))
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            else
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            ISheet sheet = wk.GetSheetAt(0);
            IRow row = null;  //读取当前行数据
            List<string> name = new List<string>();
            int index=1;
            //LastRowNum 是当前表的总行数-1（注意）
            //int offset = 0;
            fs.Close();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);  //读取当前行数据
                if (row != null)
                {
                    //LastCellNum 是当前行的总列数
                    Puzzle p = new Puzzle();
                    if (name.Contains(GetCellValue(row.GetCell(1)).ToString()))
                    {
                        //填空(1)
                        p.puzzle_name = GetCellValue(row.GetCell(1)).ToString() + "(" + index + ")";
                    }
                    else
                    {
                        index = 1;
                        name.Add(GetCellValue(row.GetCell(1)).ToString());
                        p.puzzle_name = GetCellValue(row.GetCell(1)).ToString() + "(" + index + ")";
                    }
                    p.uploadtime = DateTime.Now.ToString("yyyy-MM-dd");
                    p.puzzle_fullscore = Convert.ToDouble(GetCellValue(row.GetCell(3)).ToString());
                    list.Add(p);
                }
            }
            return list.ToArray();
        }
        public Score[] Read(string filepath)
        {
            IWorkbook wk = null;
            //Score[] scoreArray;
            List<Score> list = new List<Score>();
            string extension = Path.GetExtension(filepath);
            FileStream fs = File.OpenRead(filepath);
            if (extension.Equals(".xls"))
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            else
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            fs.Close();
            //读取当前表数据
            ISheet sheet = wk.GetSheetAt(6);
            IRow row = null;  //读取当前行数据
            //LastRowNum 是当前表的总行数-1（注意）
            //int offset = 0;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);  //读取当前行数据
                if (row != null)
                {
                    //LastCellNum 是当前行的总列数
                    Score s = new Score();
                    //string str = GetCellValue(row.GetCell(0)).ToString(); 
                    s.score_id = i - 2;
                    s.score_gradeid = GetCellValue(row.GetCell(0)).ToString();//考号
                    s.stu_name = GetCellValue(row.GetCell(1)).ToString();//姓名
                    s.stu_id = GetCellValue(row.GetCell(2)).ToString();//学号
                    s.score = Convert.ToDouble(GetCellValue(row.GetCell(4)).ToString());//成绩
                    s.updatetime = GetCellValue(row.GetCell(9)).ToString();//考试系统的更新时间
                    s.stu_class = GetCellValue(row.GetCell(12)).ToString();//考生的所属班级
                    s.stu_depart = GetCellValue(row.GetCell(13)).ToString(); //考生所属院系
                    list.Add(s);
                }
            }
            return list.ToArray();
        }

        //获取cell的数据，并设置为对应的数据类型
        private object GetCellValue(ICell cell)
        {
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date comes here
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        case CellType.Formula:
                            value = cell.CellFormula;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = "";
            }
            return value;
        }
    }
}