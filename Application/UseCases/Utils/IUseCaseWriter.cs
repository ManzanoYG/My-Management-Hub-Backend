using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Utils
{
    public interface IUseCaseWriter<out TOutput, in TInput>
    {
        TOutput Execute(TInput input);
    }
}
