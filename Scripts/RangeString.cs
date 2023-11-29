using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��Χ�ַ���
/// ��ʾ��Source�ַ����У���StartIndex��EndIndex��Χ���ַ����ɵ��ַ���
/// </summary>
public struct RangeString:IEquatable<RangeString>
{
    /// <summary>
    /// Դ�ַ���
    /// </summary>
    private string m_Source;

    /// <summary>
    /// ��ʼ����
    /// </summary>
    private int m_StartIndex;

    /// <summary>
    /// ��������
    /// </summary>
    private int m_EndIndex;

    /// <summary>
    /// ����
    /// </summary>
    private int m_Length;

    /// <summary>
    /// Դ�ַ����Ƿ�ΪNull��Empty
    /// </summary>
    private bool m_IsSourceNullOrEmpty;

    /// <summary>
    /// ��ϣ��
    /// </summary>
    private int m_HashCode;



    public RangeString(string source, int startIndex, int endIndex)
    {
        m_Source = source;
        m_StartIndex = startIndex;
        m_EndIndex = endIndex;
        m_Length = endIndex - startIndex + 1;
        m_IsSourceNullOrEmpty = string.IsNullOrEmpty(source);
        m_HashCode = 0;
    }

    public bool Equals(RangeString other)
    {
        bool isOtherNullOrEmpty = string.IsNullOrEmpty(other.m_Source);

        if (isOtherNullOrEmpty && m_IsSourceNullOrEmpty)
        {
            return true;
        }

        if (m_IsSourceNullOrEmpty || isOtherNullOrEmpty)
        {
            return false;
        }

        if (m_Length != other.m_Length)
        {
            return false;
        }

        int j = other.m_StartIndex;
        for (int i = m_StartIndex; i <= m_EndIndex; i++)
        {
            if (m_Source[i] != other.m_Source[j])
            {
                return false;
            }
            j++;
        }

        return true;
    }

    public override int GetHashCode()
    {
        if (m_HashCode == 0 && !m_IsSourceNullOrEmpty)
        {
            for (int i = m_StartIndex; i <= m_EndIndex; i++)
            {
                m_HashCode = 31 * m_HashCode + m_Source[i];
            }
        }

        return m_HashCode;
    }

    public override string ToString()
    {
        RedDotManager.Instance.CachedSb.Clear();
        for (int i = m_StartIndex; i<=m_EndIndex ;i++)
        {
            RedDotManager.Instance.CachedSb.Append(m_Source[i]);
        }

        string str = RedDotManager.Instance.CachedSb.ToString();

        return str;
    }
}
