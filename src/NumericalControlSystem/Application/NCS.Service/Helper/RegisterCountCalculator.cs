using NCS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NCS.Service.Helper
{
    public static class RegisterCountCalculator
    {
        public static int GetRegisterCount(DataType dataType)
        {
            int result = 0;

            switch (dataType)
            {
                case DataType.S16:
                    result = 1;
                    break;
                case DataType.U16:
                    result = 1;
                    break;
                case DataType.S32:
                    result = 2;
                    break;
                case DataType.U32:
                    result = 2;
                    break;
                case DataType.S64:
                    result = 4;
                    break;
                case DataType.U64:
                    break;
                case DataType.F32:
                    result = 2;
                    break;
                case DataType.D64:
                    result = 4;
                    break;
                case DataType.Bit:
                    result = 1;
                    break;
                default:
                    break;

            }

            return result;
        }
    }
}
