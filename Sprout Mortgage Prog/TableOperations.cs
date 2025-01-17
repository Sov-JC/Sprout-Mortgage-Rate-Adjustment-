﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Sprout_Mortgage_Prog
{
    class TableOperations
    {
        public enum Table
        {
            A5 = 1,
            CSA = 2,
            LA = 3,
            LLA = 4,
        }

        public enum Arm
        {
            Arm5 = 1, // 5/1 ARM
            Arm7 = 2, // 7/1 ARM 
            YearFixed30 = 3, // 30 year fixed
        }

        public enum Days
        {
            Day15 = 1,
            Day30 = 2,
            Day45 = 3,
        }

        public const int A5_TABLE_ROWS  = 18;
        public const int A5_TABLE_COLS  = 12;
        public const int CSA_TABLE_ROWS = 12;
        public const int CSA_TABLE_COLS = 10;
        public const int LA_TABLE_ROWS  = 7;
        public const int LA_TABLE_COLS  = 10;
        public const int LLA_TABLE_ROWS = 11;
        public const int LLA_TABLE_COLS = 10;


        //TODO: Rename DATA into ADJ_RATE
        // location of the adjustment rates (the data)
        // where 5/1, 7/1, and 30 year fixed start and end (inclusive)
        public const int FIVE_ONE_ARM_DATA_COL_START = 1;
        public const int FIVE_ONE_ARM_DATA_COL_END = 3;
        public const int SEVEN_ONE_ARM_DATA_COL_START = 5;
        public const int SEVEN_ONE_ARM_DATA_COL_END = 7;
        public const int THIRTY_YEAR_FIXED_DATA_COL_START = 9;
        public const int THIRTY_YEAR_FIXED_DATA_COL_END = 11;

        //location of the base rates for the A5 table
        //for 5/1 arm, 7/1 arm, and 30 year fixed.        
        public const int FIVE_ONE_ARM_BASE_RATE_COL = 0;
        public const int SEVEN_ONE_ARM_BASE_RATE_COL = 4;
        public const int THIRTY_YEAR_BASE_RATE_COL = 8;
                
        public const string A5_TABLE_NAME   = "A5 Table.csv";
        public const string CSA_TABLE_NAME  = "CSA Table.csv";
        public const string LA_TABLE_NAME   = "LA Table.csv";
        public const string LLA_TABLE_NAME  = "LLA Table.csv";

        private static String[,] a5Table = new String[A5_TABLE_ROWS,A5_TABLE_COLS];
        private static String[,] csaTable = new String[CSA_TABLE_ROWS,CSA_TABLE_COLS];
        private static String[,] laTable = new String[LA_TABLE_ROWS,LA_TABLE_COLS];
        private static String[,] llaTable = new String[LLA_TABLE_ROWS,LLA_TABLE_COLS];

        // not yet being used. Make use of them later
        private Boolean A5TableLoaded = false;
        private Boolean CSATableLoaded = false;
        private Boolean LATableLoaded = false;
        private Boolean LLATableLoaded = false;
        

        public static void loadAllTables(String dirPath)
        {
            loadA5Table(dirPath);
            printA5Table();

            loadCSATable(dirPath);
            printCSATable();

            loadLATable(dirPath);
            printLATable();

            loadLLATable(dirPath);
            printLLATable();
        }

        public static void setBaseRateComboBox(ComboBox comboBox) {
            Console.WriteLine("setBaseRateComboBox called");
            
            List<String> rates = new List<String>();
            const int RATES_ROW_START = 2;//row where rates are displayed from top to bottom
                        
            for(int i=RATES_ROW_START; i<A5_TABLE_ROWS; i++)
            {

                //the rates on the row are separated in the table by 4 cells where each column corresponds
                //to a rate in an 5/1, 7/1, or 30 year fixed column.
                const int CELL_DIFFERENCE = 4; 
                for (int j = 0; j < A5_TABLE_COLS; j += CELL_DIFFERENCE)
                {                    
                    String rate = getTableCell(i, j, Table.A5);
                    Console.WriteLine("rate: " + rate);
                    rates.Add(rate);
                }
            }

            String[] ratesSorted = rates.ToArray<String>();
            Array.Sort(ratesSorted);

            comboBox.Items.AddRange(ratesSorted);
        }
        public static void setYearPlanComboBox(ComboBox comboBox)
        {

            //TODO?: Make extraction dynamic??
            Console.WriteLine("loadYearComboBox called");
            const int YEAR_PLAN_ROW = 0;

            const int FIVE_ONE_ARM_COL = 0;
            const int SEVEN_ONE_ARM_COL = 4;
            const int THIRTY_YEAR_FIXED_COL = 8;


            string five_one_arm = a5Table[YEAR_PLAN_ROW, FIVE_ONE_ARM_COL];
            string seven_one_arm = a5Table[YEAR_PLAN_ROW, SEVEN_ONE_ARM_COL];
            string thirty_year_fixed = a5Table[YEAR_PLAN_ROW, THIRTY_YEAR_FIXED_COL];

            comboBox.Items.AddRange(new object[] { five_one_arm, seven_one_arm, thirty_year_fixed });
        }
        public static void setLTVRangeComboBox(ComboBox comboBox)
        {
            Console.WriteLine("setLTVRangeComboBox() called");

            //LTV range can be gather from many tables, let's just pick the LA table
            const int LTV_COL_START = 1; //column were the first LTV range is (0-base)
            const int LTV_COL_END = 9; //column where the last LTV range is (0-base)
            const int LTV_ROW = 0; //row where the LTV ranges lie
            int n = LTV_COL_END - LTV_COL_START + 1;

            String[] ltvValues = new string[n];
            for(int j=0; j<n; j++) {
                ltvValues[j] = laTable[LTV_ROW, (LTV_COL_START + j)];
                pstrl("ltvValues[" + j + "]: " + ltvValues[j]);
            }

            comboBox.Items.AddRange(ltvValues);
        }
        public static void setDaysComboBox(ComboBox comboBox)
        {
            String[] days = new String[] {"15", "30", "45"};
            comboBox.Items.AddRange(days);
        }
        public static void setCreditScoreRangeComboBox(ComboBox comboBox)
        {
            //TODO?: Extract dynamically?
            Console.WriteLine("setCreditScoreRangeComboBox() called");
            
            String[] creditScoreRanges = new String[]
            {
                ">=760",
                "740-759",
                "720-739",
                "700-719",
                "680-699",
                "660-679",
                "640-659",
                "620-639"
            };

            comboBox.Items.AddRange(creditScoreRanges);
        }
        public static void setLoanAmountComboBox(ComboBox comboBox)
        {
            String[] loanAmounts = new string[]
            {
                "Loan Amount < $125000",
                "Loan Amount $125000-$299999",
                "Loan Amount $300000-$749999",
                "Loan Amount $750000 - $2000000",
                "Loan Amount $2000001 - $4000000",
                "Loan Amount $4000001 - $6000000"
            };
            comboBox.Items.AddRange(loanAmounts);
        }
        public static void printA5Table()
        {
            Console.WriteLine("printA5Table() called");
            Console.WriteLine("getLength(0) is: " + a5Table.GetLength(0));
            Console.WriteLine("getLength(1) is: " + a5Table.GetLength(1));
            for (int i=0;i<a5Table.GetLength(0); i++)
            {
                for(int j=0; j<a5Table.GetLength(1); j++)
                {
                    Console.Write("[i="+i+"][j="+j+"] = " + a5Table[i, j]);
                }

                Console.WriteLine("");
            }

        }
        public static void printLATable()
        {
            
            for (int i = 0; i < laTable.GetLength(0); i++)
            {
                for (int j = 0; j < laTable.GetLength(1); j++)
                {
                    Console.Write("[i=" + i + "][j=" + j + "] = " + laTable[i, j]);
                }

                Console.WriteLine("");
            }
        }
        public static void printCSATable()
        {
            Console.WriteLine("printCSATable() called");
            for(int i=0; i<csaTable.GetLength(0); i++)
            {
                for(int j=0; j<csaTable.GetLength(1); j++)
                {
                    Console.Write("[i=" + i + "][j=" + j + "] = " + csaTable[i, j]);
                }
                Console.WriteLine("");
            }
        }
        public static void printLLATable()
        {
            for (int i = 0; i < llaTable.GetLength(0); i++)
            {
                for (int j = 0; j < llaTable.GetLength(1); j++)
                {
                    Console.Write("[i=" + i + "][j=" + j + "] = " + llaTable[i, j]);
                }
                Console.WriteLine("");
            }
        }
        public static void loadA5Table(String dirPath)
        {
            Console.WriteLine("loadA5Table called");
            string[] DELIMITERS = new string[] { "," };
            String filePath = dirPath + @"\" + A5_TABLE_NAME;
            pstrl("filePath: " + filePath);            
            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                //csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(DELIMITERS);
                csvParser.HasFieldsEnclosedInQuotes = false;


                int currentLine = 0;
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    Console.WriteLine(fields == null ? "fields null" : "fields not null");

                    int currentCol = 0;
                    foreach (String str in fields)
                    {
                        if (str == null)
                        {
                            Console.WriteLine("str is null so skipping");
                            continue;
                        }
                        Console.WriteLine(str + ", ");
                        a5Table[currentLine, currentCol++] = str;
                    }

                    currentLine++;

                    Console.WriteLine();

                }
            }
        }
        public static void loadCSATable(String dirPath) {
            Console.WriteLine("loadCSATable() called");
            
            
            String filePath = (dirPath + @"\" + CSA_TABLE_NAME); //add the file name to the dirPath
            
            string[] DELIMITERS = new string[] { "," };
            
            pstrl("filePath: " + filePath);
            
            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                //csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(DELIMITERS);
                csvParser.HasFieldsEnclosedInQuotes = false;
                int currentLine = 0;


                while (!csvParser.EndOfData)
                {
                    pstrl("currentLine:" + currentLine.ToString());

                    string[] fields = csvParser.ReadFields();

                    Console.WriteLine(fields == null ? "fields null" : "fields not null");

                    int currentCol = 0;
                    foreach (String str in fields)
                    {
                        pstrl("current col is " + currentCol.ToString());
                        if (str == null) //this doesn't happen, just remove this later
                        {
                            Console.WriteLine("str is null so skipping");
                            continue;
                        }
                        Console.WriteLine(str + ", ");
                        
                        csaTable[currentLine, currentCol++] = str;
                    }

                    currentLine++;

                    Console.WriteLine();
                }
            }
        }
        public static void loadLATable(String dirPath) {
            Console.WriteLine("loadLATable() called");
            String filePath = (dirPath + @"\" + LA_TABLE_NAME); //add the file name to the dirPath
            string[] DELIMITERS = new string[] { "," };
            pstrl("filePath: " + filePath);
            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                //csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(DELIMITERS);
                csvParser.HasFieldsEnclosedInQuotes = false;


                int currentLine = 0;
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    Console.WriteLine(fields == null ? "fields null" : "fields not null");

                    int currentCol = 0;
                    foreach (String str in fields)
                    {
                        if (str == null)
                        {
                            Console.WriteLine("str is null so skipping");
                            continue;
                        }
                        Console.WriteLine(str + ", ");
                        laTable[currentLine, currentCol++] = str;
                    }

                    currentLine++;

                    Console.WriteLine();

                }
            }
        }
        public static void loadLLATable(String dirPath) {
            Console.WriteLine("loadLLATable() called");
            String filePath = (dirPath + @"\" + LLA_TABLE_NAME); //add the file name to the dirPath
            string[] DELIMITERS = new string[] { "," };
            pstrl("filePath: " + filePath);
            using (TextFieldParser csvParser = new TextFieldParser(filePath))
            {
                //csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(DELIMITERS);
                csvParser.HasFieldsEnclosedInQuotes = false;


                int currentLine = 0;
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    Console.WriteLine(fields == null ? "fields null" : "fields not null");

                    int currentCol = 0;
                    foreach (String str in fields)
                    {
                        if (str == null)
                        {
                            Console.WriteLine("str is null so skipping");
                            continue;
                        }
                        Console.WriteLine(str + ", ");
                        llaTable[currentLine, currentCol++] = str;
                    }

                    currentLine++;

                    Console.WriteLine();

                }
            }
        }
        
        private static String getTableCell(int x, int y, Table table)
        {
            Console.WriteLine("getTableCell called");

            String cell = "";
            if (table == Table.A5)
                cell = a5Table[x, y];
            else if (table == Table.CSA)
                cell = csaTable[x, y];
            else if (table == Table.LA)
                cell = laTable[x, y];
            else if (table == Table.LLA)
                cell = llaTable[x, y];
            else
                throw new Exception("Invalid table enum type");

            Console.WriteLine("Cell is: " + cell);

            return cell;
        }

        //TODO: Rename rate to Base Rate
        //finds the adjustment rate of the A5 table for the corresponsing baserate,
        //arm, and days.
        public static String getA5AdjustmentRate(string rate, Arm arm, Days days)
        {
            pstrl("getA5AdjustmentRate() called");
            const int RATES_ROW_START = 2;//row where base rates and rate adjustments are displayed from top to bottom

            //the adj_rate_col is the one column that contains the adjustment rate. 
            //The column that the adjustment rate lies in is dependent on the Arm 
            //enum type, and the Days enum type. 
            int adj_rate_col;

            //the base_rate_col is the one column that contains the base rate that's dependent
            //on the Arm enum type.
            int base_rate_col;


            //first set the adj_rate_col to where one of the ARM types adjustment rate column starts.
            if (arm == Arm.Arm5)
            {
                base_rate_col = FIVE_ONE_ARM_BASE_RATE_COL;
            }
            else if (arm == Arm.Arm7)
            {
                base_rate_col = SEVEN_ONE_ARM_BASE_RATE_COL;
            }
            else {
                base_rate_col = THIRTY_YEAR_BASE_RATE_COL;                
            } 

            //now that the adjustment rate col's base location is known, we just offset it depending on the Days
            //enum type to find the column we want.
            if (days == Days.Day15)
                adj_rate_col = (base_rate_col + 1);
            else if (days == Days.Day30)
                adj_rate_col = base_rate_col + 2;
            else if (days == Days.Day45)
                adj_rate_col = base_rate_col + 3;
            else
                throw new Exception("Days enum type is invalid");

            String adjustmentRate = null;
            for (int i = RATES_ROW_START; i < A5_TABLE_ROWS; i++)
                if (a5Table[i, base_rate_col] != rate)
                {
                    Console.WriteLine(String.Format("a5Table[{0}, {1}] != {2}", i, base_rate_col, rate));
                    continue;
                }
                else
                {
                    Console.WriteLine("Adjustment rate found!");
                    adjustmentRate = a5Table[i, adj_rate_col];
                }

            pstrl("[adj_rate_col:" + adj_rate_col.ToString() + ",base_rate_col:" + base_rate_col + "]");

            //If no adjustment rate was found, then the input parameters to this function were incorrect.
            if (adjustmentRate == null)            
                throw new Exception("There exists no adjustment rate for the rate, ARM, and Days parameters");
            else
                return adjustmentRate;
        }

        public static string getCSAdjustmentRate(string creditScoreRange, string ltvRange)
        {   

            int row = getCsaCreditRangeRow(creditScoreRange);

            int col = getCsaLtvRangeColumn(ltvRange);
            Console.WriteLine("ltv range for csa was at col: " + col);

            return csaTable[row, col];
        }

        public static string getLAAdjustmentRate(string loanAmount, string ltvRange) {
            int row = getLALoanAmountRow(loanAmount);

            int col = getLALtvRangeCol(string ltvRange);


            return "";

        }

        private static int getLALtvRangeCol(string ltvRange)
        {
            const int LTV_RANGE_ROW = 0;//row where the ltv ranges are
            const int LTV_RANGE_COL_START = 1; //column where ltv range first appear from left to right;
            const Table TABLE = Table.LA;

            return getLtvRangeColumn(ltvRange, LTV_RANGE_ROW, LTV_RANGE_COL_START, TABLE);
        }

        private static string getLALoanAmountRow(string loanAmount)
        {



            return "";
        }

        /**get the column that 'ltvRange' lies in. 
         * 
         * ltvRow - row where the ltv ranges are
         * ltvColStart - column where the ltv ranges first appear from left to right
         * table - Table you want to find the column for
         * 
         * NOTE: DOES NOT WORK FOR LLA table. This is because the table has too many exceptions!
         */        
        private static int getLtvRangeColumn(string ltvRange, int ltvRow, int ltvColStart, Table table)
        {
            int col_loc = -1; //column where ltv range is

            int columns;//total columns in the table (used to loop through the table)
            if (table == Table.A5)
                columns = A5_TABLE_COLS;
            else if (table == Table.CSA)            
                columns = CSA_TABLE_COLS;
            else if (table == Table.LA)
                columns = LA_TABLE_COLS;
            else if (table == Table.LLA)
                throw new Exception("Invalid table input: getLtvRangeColumn does not suppose LLA table!");
            else
                throw new Exception("Invalid table input for call to getLtvRangeColumn()");

            for (int col = ltvColStart; col<columns; col++)
            {
                if (table == Table.A5)
                {
                    if (a5Table[ltvRow, col] == ltvRange)
                        col_loc = col;
                }
                else if (table == Table.CSA)
                {
                    if (csaTable[ltvRow, col] == ltvRange)
                        col_loc = col;
                }else if (table == Table.LA)
                {
                    if (csaTable[ltvRow, col] == ltvRange)
                        col_loc = col;
                }
            }

            if (col_loc == -1) //column_loc never changed -- the column was never found!
                throw new Exception("couldn't find the column where " + ltvRange);

            return col_loc;
        }

        //finds the column that the ltv range falls under
        private static int getCsaLtvRangeColumn(string ltvRange)
        {
            const int LTV_RANGE_ROW = 3;//row where the ltv ranges are
            const int LTV_RANGE_COL_START = 1;//column where ltv ranges first appear from left to right

            int col_loc = -1; //column were ltv range is

            //traverse table to find the column where 'ltvRange' is located
            for (int col = LTV_RANGE_COL_START; col < CSA_TABLE_COLS; col++)
            {
                Console.Write("csaTable[LTV_RANGE_ROW, col]: " + csaTable[LTV_RANGE_ROW, col]);

                if (ltvRange == csaTable[LTV_RANGE_ROW, col]) {
                    Console.WriteLine(" == " + ltvRange);
                    col_loc = col;
                }
                else
                {
                    Console.WriteLine(" != " + ltvRange);
                }
            }

            Console.WriteLine();

            if (col_loc== -1) //column_loc never changed -- the column was never found!
                throw new Exception("couldn't find credit score adjustment ltv range column for ltv" + ltvRange);

            return col_loc;
        }

        private static int getCsaCreditRangeRow(string creditScoreRange)
        {
            const int CS_RANGE_COL = 0; //column where credit score range lies
            const int CS_RANGE_ROW_START = 4; //column where credit score range first appears from top to bottom

            int row_loc = -1; //row where credit score range is

            //traverse table to find the row were credit score is located
            for (int row = CS_RANGE_ROW_START; row < CSA_TABLE_ROWS; row++)
                if (creditScoreRange == csaTable[row, CS_RANGE_COL])
                    row_loc = row;

            if (row_loc == -1) //column never changed -- column never found!
                throw new Exception("tried to find row where '" + creditScoreRange + "' exists but couldn't find it.");

            return row_loc;
        }

        //returns the first instance location of 'searchStr' in array 'str' from
        //'start' index to 'end' index (inclusive) 
        private static int strInArray(string[] str, int start, int end, string searchStr)
        {
            if (start < 0 || end > str.Length || (end > start))
                return -1;

            for (int i = start; i <= end; i++)
                    if (str[i] == searchStr)
                        return i;

            return -1;
        }

        private static String[] tableRowToStrArr(String[][] table, int row)
        {            
            String[] str = new string[table.GetLength(1)];
            table[row].CopyTo(str, 0);
            return str;
        }

        private static String[] tableColToStrArr(String[,] table, int col)
        {           
            if (col < 0 || col > table.GetLength(1))
                return null;
            
            List<String> strArr = new List<String>();

            for(int i=0; i<table.Length; i++)
            {
                strArr.Add(new String(table[i,col].ToCharArray()));
            }

            String[] returnArray = strArr.ToArray();
            Console.WriteLine("tableColToStrArray called on col of [length:" + table.GetLength(1) + "], the ToArray() is length: " + returnArray.Length);

            return returnArray;            
        }

        //finds a FIRST instance of a string in the row 'row' starting from 'colStart' up to 'colEnd' in 2-d array 'array'
        //returns -1 if not found or out of bounds
        //          returns the location of 'str' in the row otherwise.
        private static int strInRow(int row, string str, int colStart, int colEnd, string[,] table)
        {
            bool found = false;

            if (colEnd < colStart || row < 0 || colStart < 0)
                return -1;

            /*
            //make sure we don't clip out of bounds
            if (table == Table.A5 && (colEnd > A5_TABLE_COLS))
                return false;
            else if (table == Table.CSA && (colEnd > CSA_TABLE_COLS))
                return false;
            else if (table == Table.LA && (colEnd > LA_TABLE_COLS))
                return false;
            else if (table == Table.LLA && (colEnd > LLA_TABLE_COLS))
                return false;
                */
                
            int col_loc = -1;

            try {
                for (int col = colStart; col <= colEnd; col++)
                    if (table[row, col] == str)
                        return col_loc;
            } catch (Exception e) { 
                return -1;
            }

            return col_loc;

         }


        private static void pstr(Object o)
        {            
            Console.Write(o.ToString());
        }
        private static void pstrl(Object o)
        {
            Console.WriteLine(o.ToString());
        }

    }
}
