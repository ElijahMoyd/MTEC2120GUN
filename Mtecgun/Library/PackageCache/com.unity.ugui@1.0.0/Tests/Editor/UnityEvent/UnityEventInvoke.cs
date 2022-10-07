using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UnityEventInvoke
{
    class SimpleCounter : MonoBehaviour
    {
        public int m_Count = 0;

        public void Add()
        {
            ++m_Count;
        }

        public void NoOp(int i)
        {
        }
    }

    GameObject m_CounterObject;
    SimpleCounter Counter { get; set; }

    [SetUp]
    public void TestSetup()
    {
        m_CounterObject = new GameObject("Counter");
        Counter = m_CounterObject.AddComponent<SimpleCounter>();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(m_CounterObject);
    }

    [Test]
    [Description("Using a CachedInvokableCall in a UnityEvent should not go re-trigger all the calls stored in the UnityEvent. Case-950588")]
    public void UnityEvent_InvokeCallsListenerOnce()
    {
        var _event = new UnityEvent();
        UnityEventTools.AddPersistentListener(_event, new UnityAction(Counter.Add));
        _event.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);

        _event.Invoke();

        Assert.AreEqual(1, Counter.m_Count);

        for (int i = 1; i < 5; ++i)
        {
            UnityEventTools.AddIntPersistentListener(_event, new UnityAction<int>(Counter.NoOp), i);
            _event.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
        }

        _event.Invoke();

        Assert.AreEqual(2, Counter.m_Count);
    }
}
