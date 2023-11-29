using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 树结点
/// </summary>
public class TreeNode
{
    /// <summary>
    /// 子节点
    /// </summary>
    private Dictionary<RangeString, TreeNode> m_Children;

    /// <summary>
    /// 节点值改变回调
    /// </summary>
    private Action<int> m_ChangeCallBack;

    /// <summary>
    /// 完整路径
    /// </summary>
    private string m_FullPath;


    /// <summary>
    /// 节点名
    /// </summary>
    public string Name
    {
        get;
        private set;
    }
    
    /// <summary>
    /// 完整路径
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
    /// 节点值
    /// </summary>
    public int Value
    {
        get;
        private set;
    }

    /// <summary>
    /// 父节点
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
    /// 添加节点监听
    /// </summary>
    public void AddListener(Action<int> callback)
    {
        m_ChangeCallBack += callback;
    }

    /// <summary>
    /// 移除节点值监听
    /// </summary>
    public void RemoveListener(Action<int> callback)
    {
        m_ChangeCallBack-=callback;
    }

    /// <summary>
    /// 移除所有节点值监听
    /// </summary>
    public void RemoveAllListener()
    {
        m_ChangeCallBack=null;
    }


    /// <summary>
    /// 改变节点值（使用传入的新值，只能在叶子节点上调用）
    /// </summary>
    public void ChangeValue(int newValue)
    {
        if (m_Children!=null && m_Children.Count!=0)
        {
            throw new Exception("不能直接修改非叶子节点的值");
        }

        InterNalChangeValue(newValue);
    }

    /// <summary>
    /// 改变节点值（根据子节点值计算新值，只对非叶子节点有效）
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
    /// 获取子节点,如果不存在则添加
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
    /// 获取子节点
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
    /// 添加子节点
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
            throw new Exception("子节点添加失败，不允许重复添加");
        }

        TreeNode child = new TreeNode(key.ToString(),this);
        m_Children.Add(key,child);
        return child;
    }

    /// <summary>
    /// 移除子节点
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
            //子节点被删除，需要进行一次父节点刷新
            RedDotManager.Instance.MarkDirtyNode(this);

            m_Children.Remove(key);

            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除所有子节点
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
    /// 改变节点值
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
