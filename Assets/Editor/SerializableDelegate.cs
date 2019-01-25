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