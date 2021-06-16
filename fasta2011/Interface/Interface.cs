using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace fasta2011
{
    #region 定义接口用来刷新父窗口
    public interface IForm
    {
        
        void HideForm();
        void Suggest();
        void Suggest(DataBase _db);
        void LoadData2();
    }
    public interface IForm2
    {
        void CloseNew();
    }
    #endregion 
}
