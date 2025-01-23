using GED.Core.SanityCheck;

namespace GED.Core
{
    /// <summary>
    /// 추상 매니저 클래스 <br/>
    /// 접근 제한된 실제 데이터 리스트와 변환기를 포함함 <br/>
    /// 보유하고 있는 메모리(<see cref="list"/>)를 관리할 책임을 가짐 <br/><br/>
    /// 
    /// 사용자는 <see cref="I"/> 데이터로 가져오며,
    /// 출력값을 <see cref="O"/> 데이터로 반환받음 <br/>
    /// 이는 사용자가 <see cref="S"/> 데이터를 알 수 없게 하기 위함임 <br/><br/>
    /// 
    /// 주요 동작은 다음과 같음: <br/>
    /// - 저장된 데이터 제어 및 은닉. <br/><br/>
    /// 
    /// </summary>
    /// <typeparam name="I">
    /// <see cref="S"/> 데이터를 만들기 위한 입력 매개타입
    /// <see cref="ItoS"/>
    /// </typeparam>
    /// <typeparam name="S">
    /// 실제로 저장되는 매개타입 <br/>
    /// 현재 클래스(<see cref="Mgr(I, S, O)"/>)가 메모리 책임을 맡음 <br/><br/>
    /// 
    /// 해당 값은 <see cref="O"/> 출력값을 만드는 데 사용됨
    /// </typeparam>
    /// <typeparam name="O">
    /// 사용자에게 실제로 표시되는 출력 매개타입
    /// <see cref="StoO"/>
    /// </typeparam>
    public abstract class Mgr<I, S, O>
    {
        #region Member Fields

        /// <summary>
        /// 실제로 저장되는 데이터의 리스트
        /// </summary>
        /// <remarks>
        /// GC에게 전달되지 않도록함
        /// </remarks>
        internal List<S> list;

        #endregion

        #region Properties

        /// <summary>
        /// <see cref="list"/> 길이
        /// </summary>
        public int Length { get => list.Count; }

        #endregion

        #region Constructors

        public Mgr()
        {
            list = new();
        }

        #endregion

        #region Abstract Functions

        /// <summary>
        /// <see cref="I"/> 데이터로 <see cref="S"/> 데이터 생성
        /// </summary>
        /// <param name="inParam">
        /// 생성 매개변수
        /// (필수값)
        /// </param>
        /// <param name="outElement">
        /// 출력을 위한 버퍼값
        /// </param>
        /// <returns>
        /// <seealso cref="States"/> 에러 코드
        /// </returns>
        abstract protected int ItoS(in I inParam, out S? outElement);

        /// <summary>
        /// <see cref="S"/> 데이터로 <see cref="O"/> 데이터 생성
        /// </summary>
        /// <param name="inElement">
        /// 생성 매개변수
        /// (필수값)
        /// </param>
        /// <param name="outParam">
        /// 출력을 위한 버퍼
        /// </param>
        /// <returns>
        /// <seealso cref="States"/> 에러 코드
        /// </returns>
        abstract protected int StoO(in S inElement, out O? outParam);

        #endregion

        #region Public Functions

        /// <param name="idx">
        /// Index where the construction would occur. <br/><br/>
        /// 
        /// Passing the value greater than <see cref="Length"/> 
        /// would cause <see cref="States.WRONG_OPERATION"/>
        /// </param>
        /// 
        /// <returns>
        /// <see cref="States"/> <br/><br/>
        /// 
        /// Possibly contains the result on conversion. <br/>
        /// See <see cref="ItoS"/>.
        /// </returns>
        /// <exception cref="States.PTR_IS_NULL"/>
        /// <exception cref="States.WRONG_OPERATION"/>
        public int Emplace(int idx, in I raw)
        {
            S? s;
            int r = ItoS(in raw, out s);

            if(r != States.OK && (r & States.DONE_HOWEV) == 0)
            {
                return r;
            }

            if(s == null)
            {
                return States.PTR_IS_NULL;
            }

            if(idx >= list.Count)
            {
                return States.WRONG_OPERATION | r & ~States.DONE_HOWEV;
            }
            
            list[idx] = s;
            return r;
        }

        /// <summary>
        /// <see cref="Emplace"/> <br/><br/>
        /// 
        /// Do exactly as <see cref="Emplace"/>, 
        /// but after possibly extra allocation.
        /// </summary>
        public int EmplaceBack(in I raw)
        {
            S? s;
            int r = ItoS(in raw, out s);

            if(r != States.OK && (r & States.DONE_HOWEV) == 0)
            {
                return r;
            }

            if(s == null)
            {
                return States.PTR_IS_NULL;
            }

            list.Add(s);
            return States.OK;
        }

        /// <summary>
        /// Instantiates the new object as <see cref="O"/>
        /// and copies to <paramref name="retval"/> if non-null.
        /// </summary>
        /// <param name="index">
        /// Index for <see cref="list"/>. <br/><br/>
        /// 
        /// Given the value is greater than <see cref="Length"/>,
        /// It would cause to return <see cref="States.WRONG_OPERATION"/>.
        /// </param>
        /// <param name="retval">
        /// is a buffer for new instance. <br/><br/>
        /// 
        /// Note that <c>null</c> could be there on execution of <see cref="StoO"/>. <br/><br/>
        /// 
        /// See <see cref="StoO"/>.
        /// </param>
        /// <returns>
        /// <see cref="States"/>
        /// </returns>
        /// <exception cref="States.WRONG_OPERATION"/>
        public int GetSource(int index, out O? retval)
        {
            retval = default;

            if (index >= list.Count)
            {
                return States.WRONG_OPERATION;
            }

            S s = list[index];
            return StoO(in s, out retval);
        }

        #endregion
    }
}