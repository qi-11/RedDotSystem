using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class RedDotManager
{
    private static RedDotManager m_Instance;
    public static RedDotManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new RedDotManager();
            }
            return m_Instance;
        }
    }



    /// <summary>
    /// ���нڵ㼯��
    /// </summary>
    private Dictionary<string, TreeNode> m_AllNodes;

    /// <summary>
    /// ·���ָ��ַ�
    /// </summary>
    public char SplitChar
    {
        get;
        private set;
    }

    /// <summary>
    /// �����stringBuild
    /// </summary>
    public StringBuilder CachedSb
    {
        get;
        private set;
    }

    /// <summary>
    /// ����������
    /// </summary>
    public TreeNode Root
    {
        get; 
        private set;
    }

    /// <summary>
    /// ��ڵ㼯��
    /// </summary>
    private HashSet<TreeNode> m_DirtyNodes;

    /// <summary>
    /// ��ʱ��ڵ㼯��
    /// </summary>
    private List<TreeNode> m_TempDirtyNodes;

    public RedDotManager()
    {
        SplitChar = '/';
        m_AllNodes = new Dictionary<string, TreeNode>();
        Root=new TreeNode("Root");
        CachedSb = new StringBuilder();
        
        m_DirtyNodes = new HashSet<TreeNode>();
        m_TempDirtyNodes=new List<TreeNode>();
    }

    #region ��ڵ㴦��

    /// <summary>
    /// ��������ѯ
    /// </summary>
    public void Update()
    {
        if (m_DirtyNodes.Count==0)
        {
            return;
        }

        m_TempDirtyNodes.Clear();
        foreach (TreeNode node in m_DirtyNodes)
        {
            m_TempDirtyNodes.Add(node);
        }
        m_DirtyNodes.Clear();

        //����������ڵ�
        for (int i = 0; i < m_TempDirtyNodes.Count; i++)
        {
            m_TempDirtyNodes[i].ChangeValue();
        }
    }


    /// <summary>
    /// �����ڵ�
    /// </summary>
    public void MarkDirtyNode(TreeNode node)
    {
        if (node == null || node.Name==Root.Name)
        {
            return;
        }

        m_DirtyNodes.Add(node);
    }


    #endregion


    #region ��㴦��
    /// <summary>
    /// ��ӽڵ�ֵ����
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public TreeNode AddListener(string path, Action<int> callback)
    {
        if (callback==null)
        {
            return null;
        }

        TreeNode node =GetTreeNode(path);
        node.AddListener(callback);

        return node;
    }


    /// <summary>
    /// �Ƴ��ڵ�����
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void RemoveListener(string path, Action<int> callback)
    {
        if (callback==null)
        {
            return;
        }

        TreeNode node=GetTreeNode(path);
        node.RemoveListener(callback);
    }

    /// <summary>
    /// �Ƴ����нڵ�����
    /// </summary>
    /// <param name="path"></param>
    public void RemoveAllListener(string path)
    {
        TreeNode node=GetTreeNode(path);
        node.RemoveAllListener();
    }

    /// <summary>
    /// �ı�ڵ�ֵ
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newValue"></param>
    public void ChangeValue(string path, int newValue)
    {
        TreeNode node = GetTreeNode(path);  
        node.ChangeValue(newValue);
    }


    /// <summary>
    /// ��ȡ�ڵ�ֵ
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetValue(string path)
    {
        TreeNode node=GetTreeNode(path);
        if (node==null)
        {
            return 0;
        }

        return node.Value;
    }


    /// <summary>
    /// ��ȡ���
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public TreeNode GetTreeNode(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new Exception("·�����Ϸ�������Ϊ��");
        }

        if (m_AllNodes.TryGetValue(path,out TreeNode node))
        {
            return node;
        }

        TreeNode cur = Root;
        int length=path.Length;

        int startIndex = 0;

        for (int i = 0; i < length; i++)
        {
            if (path[i] == SplitChar)
            {
                if (i==length-1)
                {
                    throw new Exception("·�����Ϸ���������·���ָ�����β��"+path);
                }

                int endIndex = i - 1;

                if (endIndex<startIndex)
                {
                    throw new Exception("·�����Ϸ������ܴ���������·���ָ�������·���ָ�����ͷ��" + path);
                }

                TreeNode child = cur.GetOrAddChild(new RangeString(path,startIndex,endIndex));

                //����startIndex
                startIndex = i + 1;

                cur = child;
            }
        }

        //���һ���ڵ㣬ֱ����length-1 ��ΪendIndex
        TreeNode target = cur.GetOrAddChild(new RangeString(path,startIndex,length-1));

        m_AllNodes.Add(path,target);

        return target;
    }

    /// <summary>
    /// �Ƴ��ڵ�
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool RemoveTreeNode(string path)
    {
        if (!m_AllNodes.ContainsKey(path))
        {
            return false;
        }

        TreeNode node = GetTreeNode(path);
        m_AllNodes.Remove(path);
        return node.Parent.RemoveChild(new RangeString(node.Name,0,node.Name.Length-1));
    }


    /// <summary>
    /// �Ƴ����нڵ�
    /// </summary>
    public void RemoveAllTreeNode()
    {
        Root.RemoveAllChild();
        m_AllNodes.Clear();
    }
    #endregion
}
