using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �����
/// </summary>
public class TreeNode
{
    /// <summary>
    /// �ӽڵ�
    /// </summary>
    private Dictionary<RangeString, TreeNode> m_Children;

    /// <summary>
    /// �ڵ�ֵ�ı�ص�
    /// </summary>
    private Action<int> m_ChangeCallBack;

    /// <summary>
    /// ����·��
    /// </summary>
    private string m_FullPath;


    /// <summary>
    /// �ڵ���
    /// </summary>
    public string Name
    {
        get;
        private set;
    }
    
    /// <summary>
    /// ����·��
    /// </summary>
    public string FullName
    {
        get
        {
            if (string.IsNullOrEmpty(m_FullPath))
            {
                if (Parent==null || Parent == RedDotManager.Instance.Root)
                {
                    m_FullPath = Name;
                }
                else
                {
                    m_FullPath = Parent.m_FullPath + RedDotManager.Instance.SplitChar + Name;
                }
            }
            return m_FullPath;
        }
    }

    /// <summary>
    /// �ڵ�ֵ
    /// </summary>
    public int Value
    {
        get;
        private set;
    }

    /// <summary>
    /// ���ڵ�
    /// </summary>
    public TreeNode Parent
    {
        get;
        private set;
    }

    public TreeNode(string name)
    {
        Name = name;
        Value = 0;
        m_ChangeCallBack = null;
    }

    public TreeNode(string name,TreeNode parent):this(name)
    {
        Parent = parent;
    }

    /// <summary>
    /// ��ӽڵ����
    /// </summary>
    public void AddListener(Action<int> callback)
    {
        m_ChangeCallBack += callback;
    }

    /// <summary>
    /// �Ƴ��ڵ�ֵ����
    /// </summary>
    public void RemoveListener(Action<int> callback)
    {
        m_ChangeCallBack-=callback;
    }

    /// <summary>
    /// �Ƴ����нڵ�ֵ����
    /// </summary>
    public void RemoveAllListener()
    {
        m_ChangeCallBack=null;
    }


    /// <summary>
    /// �ı�ڵ�ֵ��ʹ�ô������ֵ��ֻ����Ҷ�ӽڵ��ϵ��ã�
    /// </summary>
    public void ChangeValue(int newValue)
    {
        if (m_Children!=null && m_Children.Count!=0)
        {
            throw new Exception("����ֱ���޸ķ�Ҷ�ӽڵ��ֵ");
        }

        InterNalChangeValue(newValue);
    }

    /// <summary>
    /// �ı�ڵ�ֵ�������ӽڵ�ֵ������ֵ��ֻ�Է�Ҷ�ӽڵ���Ч��
    /// </summary>
    public void ChangeValue()
    {
        int sum = 0;

        if (m_Children!=null && m_Children.Count!=0)
        {
            foreach (KeyValuePair<RangeString,TreeNode> child in m_Children)
            {
                sum += child.Value.Value;
            }
        }

        InterNalChangeValue(sum);
    }

    /// <summary>
    /// ��ȡ�ӽڵ�,��������������
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TreeNode GetOrAddChild(RangeString key)
    {
        TreeNode child=GetChild(key);
        if (child==null)
        {
            child = AddChild(key);
        }

        return child;
    }

    /// <summary>
    /// ��ȡ�ӽڵ�
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TreeNode GetChild(RangeString key)
    {
        if (m_Children==null)
        {
            return null;
        }
        m_Children.TryGetValue(key,out TreeNode child);
        return child;
    }

    /// <summary>
    /// ����ӽڵ�
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public TreeNode AddChild(RangeString key)
    {
        if (m_Children == null)
        {
            m_Children = new Dictionary<RangeString, TreeNode>();
        }
        else if(m_Children.ContainsKey(key))
        {
            throw new Exception("�ӽڵ����ʧ�ܣ��������ظ����");
        }

        TreeNode child = new TreeNode(key.ToString(),this);
        m_Children.Add(key,child);
        return child;
    }

    /// <summary>
    /// �Ƴ��ӽڵ�
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool RemoveChild(RangeString key)
    {
        if (m_Children==null || m_Children.Count==0)
        {
            return false;
        }

        TreeNode child = GetChild(key);

        if (child != null)
        {
            //�ӽڵ㱻ɾ������Ҫ����һ�θ��ڵ�ˢ��
            RedDotManager.Instance.MarkDirtyNode(this);

            m_Children.Remove(key);

            return true;
        }
        return false;
    }

    /// <summary>
    /// �Ƴ������ӽڵ�
    /// </summary>
    public void RemoveAllChild()
    {
        if (m_Children==null || m_Children.Count==0)
        {
            return;
        }
        m_Children.Clear();
        RedDotManager.Instance.MarkDirtyNode(this);
    }


    public override string ToString()
    {
        return FullName;
    }


    /// <summary>
    /// �ı�ڵ�ֵ
    /// </summary>
    /// <param name="newValue"></param>
    private void InterNalChangeValue(int newValue)
    {
        if (Value==newValue)
        {
            return;
        }

        Value = newValue;
        m_ChangeCallBack?.Invoke(Value);

        RedDotManager.Instance.MarkDirtyNode(Parent);
    }


}
