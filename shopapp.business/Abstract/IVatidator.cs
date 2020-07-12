using System.Collections.Generic;

namespace shopapp.business.Abstract
{
    public interface IVatidator<TEntity>
    {
        string ErrorMessage { get; set; }

        bool Validation(TEntity entity);
        // Dictionary<string, string> ErrorMessage;
    }
}