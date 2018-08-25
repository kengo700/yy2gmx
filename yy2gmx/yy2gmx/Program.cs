using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;

namespace yy2gmx
{
    
    class Program
    {
        static void Main(string[] args)
        {

            // 引数から読み込むファイル名を取得
            //   コマンドライン引数を配列で取得する
            //   参考：https://dobon.net/vb/dotnet/programing/dropfiletoexe.html
            string[] files = System.Environment.GetCommandLineArgs();
            if (files.Length <= 1)
            {
                return;
            }

            string font_path = files[1];
            Console.WriteLine(font_path);

            // ファイル名を取得
            string font_name = Path.GetFileNameWithoutExtension(font_path);
            // ディレクトリ名を取得
            string input_folder = Path.GetDirectoryName(font_path);

            // GameMaker Studio 2のフォントファイルの読み込み
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Font));
            FileStream fs = new FileStream(font_path, FileMode.Open);
            Font font = (Font)js.ReadObject(fs);
            fs.Close();


            // XML形式に変換
            XDocument xmldoc = new XDocument();
            XElement xfont = new XElement("font");

            xfont.Add(new XElement("name", font.fontName));
            xfont.Add(new XElement("size", font.size));
            xfont.Add(new XElement("bold", Convert.ToInt32(font.bold)));
            xfont.Add(new XElement("italic", Convert.ToInt32(font.italic)));

            XElement xrange = new XElement("ranges");
            foreach (Font.Range range in font.ranges)
            {
                xrange.Add(new XElement("range0", "" + range.x + "," + range.y));
            }
            xfont.Add(xrange);

            xrange = new XElement("glyphs");
            foreach (Font.Glyph glyph in font.glyphs)
            {
                XElement xglyph = new XElement("glyph");
                xglyph.SetAttributeValue("character", glyph.value.character);
                xglyph.SetAttributeValue("x", glyph.value.x);
                xglyph.SetAttributeValue("y", glyph.value.y);
                xglyph.SetAttributeValue("w", glyph.value.w);
                xglyph.SetAttributeValue("h", glyph.value.h);
                xglyph.SetAttributeValue("shift", glyph.value.shift);
                xglyph.SetAttributeValue("offset", glyph.value.offset);                    
                xrange.Add(xglyph);
            }
            xfont.Add(xrange);

            xfont.Add(new XElement("image", "" + font_name + ".png"));

            // XML形式で出力
            xmldoc.Add(xfont);
            StreamWriter tpag = new StreamWriter(input_folder + "\\" + font_name + ".gmx", false, System.Text.Encoding.UTF8);
            tpag.Write(xmldoc.ToString());
            tpag.Close();


        }
    }

    // GameMaster Studio 2のフォントのJSON読み込み用のクラス
    // 参考：http://posnum.hatenablog.com/entry/2014/09/19/233255
    [DataContract]
    public class Font
    {
        [DataMember]
        public string fontName { get; set; }
        [DataMember]
        public int size { get; set; }
        [DataMember]
        public bool bold { get; set; }
        [DataMember]
        public bool italic { get; set; }

        [DataMember]
        public Glyph[] glyphs { get; set; }
        [DataContract]
        public class Glyph
        {
            [DataMember(Name = "Value")]
            public Value value { get; set; }

            [DataContract]
            public class Value
            {
                [DataMember]
                public int character { get; set; }
                [DataMember]
                public int x { get; set; }
                [DataMember]
                public int y { get; set; }
                [DataMember]
                public int w { get; set; }
                [DataMember]
                public int h { get; set; }
                [DataMember]
                public int shift { get; set; }
                [DataMember]
                public int offset { get; set; }
            }
        }

        [DataMember]
        public Range[] ranges { get; set; }
        [DataContract]
        public class Range
        {
            [DataMember]
            public int x { get; set; }
            [DataMember]
            public int y { get; set; }

        }
    }

}
