using System;

namespace Wugner.OpenXml
{
    public class ExcelCell
	{
        public string Header { get; set; }

        private string _content;
        private int _row;
        private int _column;

        public string StringValue { get { return this.ToStringValue(); } }
        public bool BoolValue { get { return this.ToBoolValue(); } }
        public int IntValue { get { return this.ToIntValue(); } }
        public float FloatValue { get { return this.ToFloatValue(); } }
        public DateTime DateValue { get { return this.ToDateValue(); } }

        public ExcelCell(string content, int row, int column)
        {
            this._content = content;
            this._row = row;
            this._column = column;
        }

        private string ToStringValue()
        {
            return this._content; 
        }

        private bool ToBoolValue()
        {
            if (string.IsNullOrEmpty(this._content))
            {
                return false; 
            }
            if (this._content == "0")
            {
                return false;
            } else if (this._content == "1")
            {
                return true; 
            }
            if (this._content.ToLower() == "false")
            {
                return false;
            } else if (this._content.ToLower() == "true")
            {
                return true; 
            }

            throw new Exception(this.GetParseExceptionFormatString("bool"));
        }

        private int ToIntValue()
        {
            int ret = 0;
            try
            {
                ret = int.Parse(this._content);
            } catch (Exception e)
            {
                throw new Exception(this.GetParseExceptionFormatString("int"), e);
            }
            return ret;
        }

        private float ToFloatValue()
        {
            float ret = 0;
            try
            {
                ret = float.Parse(this._content);
            } catch (Exception e)
            {
                throw new Exception(this.GetParseExceptionFormatString("float"), e);
            }
            return ret;
        }

        private DateTime ToDateValue()
        {
            DateTime ret = DateTime.UtcNow;
            try
            {
                ret = DateTime.Parse(this._content);
            } catch (Exception e)
            {
                throw new Exception(this.GetParseExceptionFormatString("DateTime"), e);
            }
            return ret;
        }

        private string GetParseExceptionFormatString(string type_string)
        {
            return string.Format("Can not parse {1} to {0} for filed {2} at [row:{3}, column:{4}]", type_string, this._content, this.Header, this._row, this._column);
        }

        public static implicit operator string(ExcelCell c) 
        {
            return c.StringValue;
        }

        public override string ToString()
        {
            return string.Format("ExcelCell:[header:{0}, content:{1}, row:{3}, column:{4}]", this.Header, this._content, this._row, this._column); ;
        }

        public void SetValue(string v)
        {
            this._content = v;
        }
    }	
}