using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Utils
{
    public interface IUseCaseParameterizeQuery<out TOutput, in TParam>
    {
        TOutput Execute(TParam param);
    }
    public interface IUseCaseParameterizeQuery<out TOutput, in TParam, in TParam2>
    {
        TOutput Execute(TParam param, TParam2 param2);
    }
    public interface IUseCaseParameterizeQuery<out TOutput, in TParam, in TParam2, in TParam3>
    {
        TOutput Execute(TParam param, TParam2 param2, TParam3 param3);
    }
}
