using GED.Core.SanityCheck;

namespace GED.Core
{
    /// <summary>
    /// �߻� �Ŵ��� Ŭ���� <br/>
    /// ���� ���ѵ� ���� ������ ����Ʈ�� ��ȯ�⸦ ������ <br/>
    /// �����ϰ� �ִ� �޸�(<see cref="list"/>)�� ������ å���� ���� <br/><br/>
    /// 
    /// ����ڴ� <see cref="I"/> �����ͷ� ��������,
    /// ��°��� <see cref="O"/> �����ͷ� ��ȯ���� <br/>
    /// �̴� ����ڰ� <see cref="S"/> �����͸� �� �� ���� �ϱ� ������ <br/><br/>
    /// 
    /// �ֿ� ������ ������ ����: <br/>
    /// - ����� ������ ���� �� ����. <br/><br/>
    /// 
    /// </summary>
    /// <typeparam name="I">
    /// <see cref="S"/> �����͸� ����� ���� �Է� �Ű�Ÿ��
    /// <see cref="ItoS"/>
    /// </typeparam>
    /// <typeparam name="S">
    /// ������ ����Ǵ� �Ű�Ÿ�� <br/>
    /// ���� Ŭ����(<see cref="Mgr(I, S, O)"/>)�� �޸� å���� ���� <br/><br/>
    /// 
    /// �ش� ���� <see cref="O"/> ��°��� ����� �� ����
    /// </typeparam>
    /// <typeparam name="O">
    /// ����ڿ��� ������ ǥ�õǴ� ��� �Ű�Ÿ��
    /// <see cref="StoO"/>
    /// </typeparam>
    public abstract class Mgr<I, S, O>
    {
        #region Member Fields

        /// <summary>
        /// ������ ����Ǵ� �������� ����Ʈ
        /// </summary>
        /// <remarks>
        /// GC���� ���޵��� �ʵ�����
        /// </remarks>
        internal List<S> list;

        #endregion

        #region Properties

        /// <summary>
        /// <see cref="list"/> ����
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
        /// <see cref="I"/> �����ͷ� <see cref="S"/> ������ ����
        /// </summary>
        /// <param name="inParam">
        /// ���� �Ű�����
        /// (�ʼ���)
        /// </param>
        /// <param name="outElement">
        /// ����� ���� ���۰�
        /// </param>
        /// <returns>
        /// <seealso cref="States"/> ���� �ڵ�
        /// </returns>
        abstract protected int ItoS(in I inParam, out S? outElement);

        /// <summary>
        /// <see cref="S"/> �����ͷ� <see cref="O"/> ������ ����
        /// </summary>
        /// <param name="inElement">
        /// ���� �Ű�����
        /// (�ʼ���)
        /// </param>
        /// <param name="outParam">
        /// ����� ���� ����
        /// </param>
        /// <returns>
        /// <seealso cref="States"/> ���� �ڵ�
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