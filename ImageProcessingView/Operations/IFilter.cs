using ImageProcessingCore.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingView.Operations
{
    public interface IFilter
    {
        IFilterStrategy GetOperationStrategy();
    }
}
