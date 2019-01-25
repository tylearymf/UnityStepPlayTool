//参考自http://schemingdeveloper.com/2014/12/04/serializing-delegates-unity/
//Delegate没有Serializable标记，但是可以序列化的原因是https://referencesource.microsoft.com/#mscorlib/system/rttype.cs,3830130f2931fca3
/*
 * 看BinaryFormatter源码时发现会调用该方法判断是否可序列化
 * internal bool IsSpecialSerializableType()
        {
            RuntimeType rt = this;
            do
            {
                // In all sane cases we only need to compare the direct level base type with
                // System.Enum and System.MulticastDelegate. However, a generic argument can
                // have a base type constraint that is Delegate or even a real delegate type.
                // Let's maintain compatibility and return true for them.
                if (rt == RuntimeType.DelegateType || rt == RuntimeType.EnumType)
                    return true;
 
                rt = rt.GetBaseType();
            } while (rt != null);
 
            return false;
        }
 * */


using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SerializableDelegate : ISerializationCallbackReceiver
{
    [SerializeField]
    UnityEngine.Object mTarget;
    [SerializeField]
    string mMethodName;
    [SerializeField]
    byte[] mSerialData;
    [SerializeField]
    string mDelegateTypeFullName;

    object mValue;
    public object value
    {
        get
        {
            return mValue;
        }
        private set
        {
            mValue = value;
        }
    }

    public SerializableDelegate(object pValue)
    {
        if (pValue == null) return;

        var tType = pValue.GetType();
        if (!tType.IsSubclassOf(typeof(Delegate)))
        {
            throw new InvalidOperationException(pValue.GetType().Name + " is not a delegate type.");
        }

        mDelegateTypeFullName = tType.FullName;
        value = pValue;
    }

    void Serialize(object pValue)
    {
        if (pValue == null)
        {
            mTarget = null;
            mMethodName = "";
            mSerialData = null;
            return;
        }
         
        var tDelegate = pValue as Delegate;
        if (tDelegate == null)
        {
            throw new InvalidOperationException(pValue.GetType().Name + " is not a delegate type.");  
        }

        mTarget = tDelegate.Target as Object;

        if (mTarget != null)
        {
            mMethodName = tDelegate.Method.Name;
            mSerialData = null;
        }
        else
        {
            //Serialize the data to a binary stream
            using (var tStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(tStream, pValue);
                tStream.Flush();
                mSerialData = tStream.ToArray();
            }
            mMethodName = null;
        }
    }

    object Deserialize()
    {
        if ((mSerialData == null || mSerialData.Length == 0) && string.IsNullOrEmpty(mMethodName))
        {
            return null;
        }

        if (mTarget != null)
        {
            return Delegate.CreateDelegate(Type.GetType(mDelegateTypeFullName), mTarget, mMethodName);
        }

        using (var tStream = new MemoryStream(mSerialData))
        {
            return (new BinaryFormatter()).Deserialize(tStream);
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        Serialize(mValue);
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        value = Deserialize();
    }
}