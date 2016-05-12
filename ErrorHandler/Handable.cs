using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandler
{
    public class Handable
    {
        private bool first = true;
        private Exception error = null;
        private List<Object> values = new List<Object>();
        private List<Object> handlers = new List<Object>();
        private List<Action> errorObservers = new List<Action>();
        private List<Action<Exception>> errorWithParamObservers = new List<Action<Exception>>();
        private List<Action> sucessObservers = new List<Action>();
        private List<Object> sucessWithParamObservers = new List<Object>();
        private Object retorno;
        

        public Handable ForValue(Object value)
        {   
            values.Add(value);
            return this;
        }       

        public Handable ForFunction<R,P1>(Func<P1,R> fn)
        {
            handlers.Add(fn);
            return this;
        }
        
        public Handable ForFunction<R,P1,P2>(Func<P1,P2,R> fn)
        {
            handlers.Add(fn);
            return this;
        }
        
        public Handable ForFunction<R,P1,P2,P3>(Func<P1,P2,P3,R> fn)
        {
            handlers.Add(fn);
            return this;
        }

        public Handable OnError(Action fn) 
        {
            errorObservers.Add(fn);
            return this;
        }
        public Handable OnError(Action<Exception> fn)
        {
            errorWithParamObservers.Add(fn);
            return this;
        }
        public Handable OnSucess(Action fn)
        {
            sucessObservers.Add(fn);
            return this;
        }

        public Handable OnSucess<R>(Action<R> fn)
        {
            sucessWithParamObservers.Add(fn);
            return this;
        }

        #region Executar funcao com 1 parametro
        public Handable Execute<R, P1>()
        {
            if (error!=null) return this;
            try
            {
              P1 param = (first) ? (P1)values.ElementAt(0) : (P1)retorno;

              first = false;

              retorno = ((Func<P1,R>)handlers.ElementAt(0))(param);     
          
              handlers.RemoveAt(0);
               
              

            }
            catch (Exception ex)
            {
                error = ex;
            }
            return this;
        }
        #endregion

        #region Executar funcao com 2 parametro
        public Handable Execute<R, P1, P2>()
        {
            if (!first) throw new Exception("Invocacao ilegal no Handable !");
            first = false;
            try
            {           
                retorno = ((Func<P1,P2, R>)handlers.ElementAt(0))
                            ((P1)values.ElementAt(0), (P2)values.ElementAt(1));

                handlers.RemoveAt(0);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return this;
        }
        #endregion

        #region Executar funcao com 3 parametro
        public Handable Execute<R, P1, P2, P3>()
        {
            if (!first) throw new Exception("Invocacao ilegal no Handable !");
            first = false;
            try
            {
                retorno = ((Func<P1,P2,P3, R>)handlers.ElementAt(0))
                            ((P1)values.ElementAt(0), (P2)values.ElementAt(1), (P3)values.ElementAt(2));

                handlers.RemoveAt(0);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return this;
        }
        #endregion
        #region Executar funcao com um parametro externo
        public Handable Execute<R, P1, P2>(P2 param2)
        {
            if (error!=null) return this;
            first = false;
            try
            {
                retorno = ((Func<P1, P2, R>)handlers.ElementAt(0))
                            ((P1) retorno,param2);

                handlers.RemoveAt(0);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return this;
        }
        #endregion
        public R Finish<R>(R def)
        {
            if(error==null)
            {
                try
                {
                    NotifySucess<R>((R)retorno);
                    return (R)retorno;
                }
                catch (Exception ex)
                {
                    error = new Exception(ex.Message + " Erro executando OnSucess", ex);
                    NotifyError();
                    return def;
                }
            }
            else
            {
                try
                {
                    NotifyError();
                }
                catch (Exception ex)
                {
                    //Do not throw any exception. But Logs it
                    Logger.GetInstance().LogError("SEVERO : Excecao no metodo OnError ! " + ex.Message);
                }
                return def;
            }
           
        }
        //Aciona todos "onError"
        private void NotifyError()
        {
            foreach (Action fn in errorObservers)
            {
                fn();
            }
            foreach (Action<Exception> fn in errorWithParamObservers)
            {
                fn(error);
            }
        }

        //Aciona todos "onSucess"
        private void NotifySucess<R>(R param)
        { 
            foreach (Action<R> fn in sucessWithParamObservers)
            {
                fn(param);
            }

            foreach (Action fn in sucessObservers)
            {
                fn();
            }
        }
    }
}
