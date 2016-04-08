using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDotNet.NativeLibrary;
using RDotNet;
using Integracion_R.Clases;
namespace Integracion_R
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Prueba()
        {
            string rhome = System.Environment.GetEnvironmentVariable("R_HOME");
            string rPath = "";
            if (string.IsNullOrEmpty(rhome))
            {
                rhome = @"C:\Program Files\R\R-3.2.4revised";
                rPath = rhome + @"\bin\i386";
                System.Environment.SetEnvironmentVariable("R_HOME", rhome);
                System.Environment.SetEnvironmentVariable("PATH", rPath);
            }
            REngine engine = REngine.GetInstance();

            CharacterVector path_texto = engine.CreateCharacterVector(new[] { "C:\\Users\\kibernum\\Desktop\\Titulos_Proyectos.txt" });
            engine.SetSymbol("ubicacion", path_texto);
            engine.Evaluate("plain.text = read.table (ubicacion,sep='\r')");

            engine.Evaluate("library(NLP)");
            engine.Evaluate("library(tm)");

            engine.Evaluate("myCorpus = Corpus(VectorSource(plain.text))");
            engine.Evaluate("myCorpus = tm_map(myCorpus, tolower)");
            engine.Evaluate("myCorpus = tm_map(myCorpus, removePunctuation)");
            engine.Evaluate("myCorpus = tm_map(myCorpus, removeNumbers)");
            engine.Evaluate("myCorpus = tm_map(myCorpus, removeWords, stopwords('spanish'))");

            engine.Evaluate("myCorpus <- tm_map(myCorpus, PlainTextDocument)");
            engine.Evaluate("myDTM = TermDocumentMatrix(myCorpus, control = list(minWordLength = 1))");
            engine.Evaluate("m = as.matrix(myDTM)");
            engine.Evaluate("myDF <- cbind(Words = rownames(as.data.frame(m)), as.data.frame(m))"); //convierto matriz "m" a DataFrame
            engine.Evaluate("matrix1 <- as.matrix(myDF)");
            DataFrame mat = engine.GetSymbol("myDF").AsDataFrame();
            CharacterMatrix mat1 = engine.GetSymbol("matrix1").AsCharacterMatrix();

            for (int i = 0; i < mat1.RowCount; ++i)
            {
                string word = Convert_UTF8_Unicode(mat1[i, 0]);
                int freq = Convert.ToInt32(mat1[i, 1]);
            }
        }

        private string Convert_UTF8_Unicode(string UTF8) 
        {
            // Turn string back to bytes using the original, incorrect encoding.
            byte[] bytes = Encoding.GetEncoding(1252).GetBytes(UTF8);
            // Use the correct encoding this time to convert back to a string.
           return Encoding.UTF8.GetString(bytes);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Prueba();
        }
    }
}
