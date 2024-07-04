/* 
 * ================================================
 * Describe:      This script is used to enhance the scroll view, copy the UGUI source code also refer to Wenruo code. 
 * Author:        Xiaohei.Wang(Wenhao)
 * CreationTime:  2023-01-28 11:23:34
 * ModifyAuthor:  Xiaohei.Wang(Wenhao)
 * ModifyTime:    2023-01-28 11:23:34
 * ScriptVersion: 0.1
 * ===============================================
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyFramework.UI
{
    /// <summary>
    /// A super component for making a child RectTransform scroll.
    /// </summary>
    /// <remarks>
    /// ScrollRectPro will not do any clipping on its own. Combined with a Mask component, it can be turned into a scroll view.
    /// </remarks>
    [AddComponentMenu("UI/Scroll Rect Pro", 102)]
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ScrollRectPro : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
    {
        /// <summary>
        /// A setting for which behavior to use when content moves beyond the confines of its container.
        /// <para>�����ݳ����������ķ�Χʱʹ�õ���Ϊ���á�</para>
        /// </summary>
        public enum MovementType
        {
            /// <summary>
            /// Unrestricted movement. The content can move forever.
            /// </summary>
            Unrestricted,

            /// <summary>
            /// Elastic movement. The content is allowed to temporarily move beyond the container, but is pulled back elastically.
            /// </summary>
            Elastic,

            /// <summary>
            /// Clamped movement. The content can not be moved beyond its container.
            /// </summary>
            Clamped,
        }

        public AxisType direction = AxisType.Vertical;

        public MovementType movementType = MovementType.Elastic;

        /// <summary>
        /// The content that can be scrolled. It should be a child of the GameObject.
        /// <para>�ɹ��������ݡ���Ӧ���ǹ�����ͼ���Ӷ���������ScrollRectPro��</para>
        /// </summary>
        public RectTransform content;

        /// <summary>
        /// The elemental of content that can be scrolled.
        /// <para>�������ݵ�Ԫ��</para>
        /// </summary>
        public GameObject Elemental;

        /// <summary>
        /// Current scroll view pro element max count.
        /// <para>��ǰ������ͼԪ�ص���������</para>
        /// </summary>
        public int ElementMaxCount => m_MaxElementalCount;

        /// <summary>
        /// The current velocity of the content.
        /// <para>��ǰ�Ĺ����ٶ�</para>
        /// </summary>
        /// <remarks>
        /// The velocity is defined in units per second.
        /// <para>�ٶȵĵ�λ��ÿ�롣</para>
        /// </remarks>
        public Vector2 Velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        /// <summary>
        /// The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.
        /// <para>����λ�ã�����(0,0)��(1,1)֮�䣬����(0,0)�����½ǡ�</para>
        /// </summary>
        public Vector2 NormalizedPosition
        {
            get
            {
                return new Vector2(HorizontalNormalizedPosition, VerticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }

        /// <summary>
        /// The horizontal scroll position as a value between 0 and 1, with 0 being at the left.
        /// <para>ˮƽ����λ��Ϊ0��1֮���ֵ��0��ʾ��ࡣ</para>
        /// </summary>
        public float HorizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if ((m_ContentBounds.size.x <= m_ViewBounds.size.x) || Mathf.Approximately(m_ContentBounds.size.x, m_ViewBounds.size.x))
                    return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
                return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }

        /// <summary>
        /// The vertical scroll position as a value between 0 and 1, with 0 being at the bottom.
        /// <para>��ֱ����λ��Ϊ0��1֮���ֵ��0��ʾ�ײ���</para>
        /// </summary>
        public float VerticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if ((m_ContentBounds.size.y <= m_ViewBounds.size.y) || Mathf.Approximately(m_ContentBounds.size.y, m_ViewBounds.size.y))
                    return (m_ViewBounds.min.y > m_ContentBounds.min.y) ? 1 : 0;

                return (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }

        private ScrollRectPro() { }

        #region SerializeField
        [SerializeField]
        int m_Lines = 1;

        [SerializeField]
        bool m_Inertia = true;

        [SerializeField]
        float m_DockSpeed = 20f;

        [SerializeField]
        bool m_AutoDocking = false;

        [SerializeField]
        int m_MaxCount = 10;

        /// <summary>
        /// The amount of elasticity to use when the content moves beyond the scroll rect.
        /// </summary>
        [SerializeField]
        float m_Elasticity = 0.1f;

        /// <summary>
        /// The rate at which movement slows down.
        /// *****Only used when inertia is enabled*****
        /// </summary>
        /// <remarks>
        /// The deceleration rate is the speed reduction per second. A value of 0.5 halves the speed each second. The default is 0.135. The deceleration rate is only used when inertia is enabled.
        /// </remarks>
        [SerializeField]
        float m_DecelerationRate = 0.135f;

        /// <summary>
        /// The sensitivity to scroll wheel and track pad scroll events.
        /// </summary>
        /// <remarks>
        /// Higher values indicate higher sensitivity.
        /// </remarks>
        [SerializeField]
        float m_ScrollSensitivity = 1.0f;

        /// <summary>
        /// Horizontal and vertical spacing
        /// </summary>
        [SerializeField]
        Vector2Int m_Spacing = new Vector2Int(10, 10);

        #endregion

        #region Local Field
        private int m_MinIndex = -1;
        private int m_MaxIndex = -1;
        private int m_CurrentIndex = -1;
        private int m_MaxElementalCount = -1;

        private float m_ElementWidth;
        private float m_ElementHeight;
        private float m_ContentWidth;
        private float m_ContentHeight;

        private bool m_Dragging;
        private bool m_Scrolling;
        private bool m_Inited = false;

        private Bounds m_ViewBounds;
        private Bounds m_ContentBounds;
        private Bounds m_PrevViewBounds;
        private Bounds m_PrevContentBounds;

        private Vector2 m_Velocity;
        private Vector2 m_PrevPosition = Vector2.zero;
        private Vector2 m_ContentStartPosition = Vector2.zero;
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;

        private Vector3[] m_Corners = new Vector3[4];

        private RectTransform m_rect;
        private RectTransform m_Rect
        {
            get
            {
                if (m_rect == null)
                    m_rect = GetComponent<RectTransform>();
                return m_rect;
            }
        }

        private struct ElementInfo
        {
            public Vector3 Postation;
            public GameObject Element;
        };
        private ElementInfo[] m_ElementInfosArray;
        private Stack<GameObject> m_ElementsPool;

        private Action<GameObject, int> CallbackFunc;
        #endregion

        public override bool IsActive()
        {
            return base.IsActive() && content != null;
        }

        protected virtual void LateUpdate()
        {
            if (!content)
                return;

            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
            {
                int _axis = (int)direction;
                Vector2 position = content.anchoredPosition;
                // Apply spring physics if movement is elastic and content has an offset from the view.
                if (movementType == MovementType.Elastic && offset[_axis] != 0)
                {
                    float speed = m_Velocity[_axis];
                    float smoothTime = m_Elasticity;
                    if (m_Scrolling)
                        smoothTime *= 3.0f;
                    position[_axis] = Mathf.SmoothDamp(content.anchoredPosition[_axis], content.anchoredPosition[_axis] + offset[_axis], ref speed, smoothTime, Mathf.Infinity, deltaTime);
                    if (Mathf.Abs(speed) < 1)
                        speed = 0;
                    m_Velocity[_axis] = speed;
                }
                // Else move content according to velocity with deceleration applied.
                else if (m_Inertia)
                {
                    m_Velocity[_axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
                    if (Mathf.Abs(m_Velocity[_axis]) < 1)
                        m_Velocity[_axis] = 0;
                    position[_axis] += m_Velocity[_axis] * deltaTime;
                }
                // If we have neither elaticity or friction, there shouldn't be any velocity.
                else
                {
                    m_Velocity[_axis] = 0;
                }

                if (movementType == MovementType.Clamped)
                {
                    offset = CalculateOffset(position - content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }

            if (m_Dragging && m_Inertia)
            {
                Vector3 newVelocity = (content.anchoredPosition - m_PrevPosition) / deltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || content.anchoredPosition != m_PrevPosition)
            {
                UISystemProfilerApi.AddMarker("ScrollRectPro.value", this);
                UpdateCheck(NormalizedPosition);
                UpdatePrevData();
            }

            if (!m_Dragging && m_AutoDocking)
            {
                float _va = m_Velocity[(int)direction];
                if (Mathf.Abs(_va) <= m_DockSpeed && Mathf.Abs(_va) != 0)
                {
                    m_Velocity = Vector2.zero;
                    if (_va < 0)
                    {
                        if (direction == AxisType.Horizontal)
                            GoToElementPosWithIndex(m_CurrentIndex - m_MaxIndex);
                        else
                            GoToElementPosWithIndex(m_CurrentIndex + 1);
                    }
                    else
                    {
                        if (direction == AxisType.Horizontal)
                            GoToElementPosWithIndex(m_CurrentIndex + 1);
                        else
                            GoToElementPosWithIndex(m_CurrentIndex - m_MaxIndex);
                    }
                }
            }
            m_Scrolling = false;
        }

        protected override void OnDisable()
        {
            m_Dragging = false;
            m_Scrolling = false;
            CallbackFunc = null;
            m_Velocity = Vector2.zero;
        }

        void IScrollHandler.OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            if (direction == AxisType.Vertical)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    delta.y = delta.x;
                delta.x = 0;
            }
            if (direction == AxisType.Horizontal)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    delta.x = delta.y;
                delta.y = 0;
            }

            if (data.IsScrolling())
                m_Scrolling = true;

            Vector2 position = content.anchoredPosition;
            position += delta * m_ScrollSensitivity;
            if (movementType == MovementType.Clamped)
                position += CalculateOffset(position - content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        #region Drag Handler
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(((RectTransform)transform), eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = content.anchoredPosition;
            m_Dragging = true;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!m_Dragging)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(((RectTransform)transform), eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
                return;

            UpdateBounds();

            var _pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 _position = m_ContentStartPosition + _pointerDelta;

            // Offset to get content into place in the view.
            Vector2 _offset = CalculateOffset(_position - content.anchoredPosition);
            _position += _offset;
            if (movementType == MovementType.Elastic)
            {
                if (_offset.x != 0)
                    _position.x = _position.x - RubberDelta(_offset.x, m_ViewBounds.size.x);
                if (_offset.y != 0)
                    _position.y = _position.y - RubberDelta(_offset.y, m_ViewBounds.size.y);
            }

            SetContentAnchoredPosition(_position);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        #endregion

        #region Private Fcuntion
        /// <summary>
        /// Sets the anchored position of the content.
        /// <para>�������ݵ�ê��λ�á�</para>
        /// </summary>
        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (direction == AxisType.Vertical)
                position.x = content.anchoredPosition.x;
            if (direction == AxisType.Horizontal)
                position.y = content.anchoredPosition.y;

            if (position != content.anchoredPosition)
            {
                content.anchoredPosition = position;
                UpdateBounds();
            }
        }

        /// <summary>
        /// Helper function to update the previous data fields on a ScrollRectPro. Call this before you change data in the ScrollRectPro.
        /// <para>��ScrollRectPro�ϸ���֮ǰ�������ֶεĸ����������ڸ���ScrollRectPro�е�����֮ǰ��������</para>
        /// </summary>
        protected void UpdatePrevData()
        {
            if (content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        /// <summary>
        /// >Set the horizontal or vertical scroll position as a value between 0 and 1, with 0 being at the left or at the bottom.
        /// <para>��ˮƽ��ֱ����λ������Ϊ0��1֮���ֵ��0��ʾ����ײ���</para>
        /// </summary>
        /// <param name="value">The position to set, between 0 and 1.<para>Ҫ���õ�λ�ã���0��1֮�䡣</para></param>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.<para>Ҫ���õ���:0��ʾˮƽ��1��ʾ��ֱ��</para></param>
        protected virtual void SetNormalizedPosition(float value, int axis)
        {
            UpdateBounds();
            // How much the content is larger than the view.
            float hiddenLength = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
            // Where the position of the lower left corner of the content bounds should be, in the space of the view.
            float contentBoundsMinPosition = m_ViewBounds.min[axis] - value * hiddenLength;
            // The new content localPosition, in the space of the view.
            float newLocalPosition = content.localPosition[axis] + contentBoundsMinPosition - m_ContentBounds.min[axis];

            Vector3 localPosition = content.localPosition;
            if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            {
                localPosition[axis] = newLocalPosition;
                content.localPosition = localPosition;
                m_Velocity[axis] = 0;
                UpdateBounds();
            }
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }

        /// <summary>
        /// Calculate the bounds the ScrollRectPro should be using.
        /// <para>����ScrollRectProӦ��ʹ�õı߽硣</para>
        /// </summary>
        protected void UpdateBounds()
        {
            m_ViewBounds = new Bounds(((RectTransform)transform).rect.center, ((RectTransform)transform).rect.size);
            m_ContentBounds = GetBounds();

            if (content == null)
                return;

            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            var contentPivot = content.pivot;
            AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;

            if (movementType == MovementType.Clamped)
            {
                Vector2 delta = Vector2.zero;
                if (m_ViewBounds.max.x > m_ContentBounds.max.x)
                {
                    delta.x = Math.Min(m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
                }
                else if (m_ViewBounds.min.x < m_ContentBounds.min.x)
                {
                    delta.x = Math.Max(m_ViewBounds.min.x - m_ContentBounds.min.x, m_ViewBounds.max.x - m_ContentBounds.max.x);
                }

                if (m_ViewBounds.min.y < m_ContentBounds.min.y)
                {
                    delta.y = Math.Max(m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
                }
                else if (m_ViewBounds.max.y > m_ContentBounds.max.y)
                {
                    delta.y = Math.Min(m_ViewBounds.min.y - m_ContentBounds.min.y, m_ViewBounds.max.y - m_ContentBounds.max.y);
                }
                if (delta.sqrMagnitude > float.Epsilon)
                {
                    contentPos = content.anchoredPosition + delta;
                    if (direction == AxisType.Vertical)
                        contentPos.x = content.anchoredPosition.x;
                    if (direction == AxisType.Horizontal)
                        contentPos.y = content.anchoredPosition.y;
                    AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
                }
            }
        }

        /// <summary>
        /// ������Χ
        /// </summary>
        /// <param name="viewBounds">��ͼ�߽�</param>
        /// <param name="contentPivot">�������ĵ�</param>
        /// <param name="contentSize">���ݳߴ�</param>
        /// <param name="contentPos">����λ��</param>
        void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
        {
            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x = viewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y = viewBounds.size.y;
            }
        }

        /// <summary>
        /// ��ȡ��Χ
        /// </summary>
        /// <returns></returns>
        private Bounds GetBounds()
        {
            if (content == null)
                return new Bounds();
            content.GetWorldCorners(m_Corners);
            var viewWorldToLocalMatrix = ((RectTransform)transform).worldToLocalMatrix;

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        /// <summary>
        /// ����ƫ����
        /// </summary>
        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if (movementType == MovementType.Unrestricted)
                return offset;

            Vector2 min = m_ContentBounds.min;
            Vector2 max = m_ContentBounds.max;

            if (direction == AxisType.Horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;

                float maxOffset = m_ViewBounds.max.x - max.x;
                float minOffset = m_ViewBounds.min.x - min.x;

                if (minOffset < -0.001f)
                    offset.x = minOffset;
                else if (maxOffset > 0.001f)
                    offset.x = maxOffset;
            }
            if (direction == AxisType.Vertical)
            {
                min.y += delta.y;
                max.y += delta.y;

                float maxOffset = m_ViewBounds.max.y - max.y;
                float minOffset = m_ViewBounds.min.y - min.y;

                if (maxOffset > 0.001f)
                    offset.y = maxOffset;
                else if (minOffset < -0.001f)
                    offset.y = minOffset;
            }
            return offset;
        }

        /// <summary>
        /// �Ӷ�����л�ȡһ������
        /// </summary>
        protected GameObject GetPoolsObj()
        {
            GameObject _go = null;
            if (m_ElementsPool.Count > 0)
                _go = m_ElementsPool.Pop();
            if (_go == null)
                _go = Instantiate(Elemental);

            _go.transform.SetParent(content.transform);
            _go.transform.localScale = Vector3.one;
            SetActive(_go, true);

            return _go;
        }

        /// <summary>
        /// Set the element push the pool.
        /// <para>����һ��Ԫ�ض���</para>
        /// </summary>
        protected void SetPoolsObj(GameObject element)
        {
            if (element != null)
            {
                m_ElementsPool.Push(element);
                SetActive(element, false);
            }
        }

        /// <summary>
        /// Set active with the element.
        /// <para>����Ԫ�ؿɼ�״̬</para>
        /// </summary>
        protected void SetActive(GameObject obj, bool isActive)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }

        /// <summary>
        /// Check the anchor is right.
        /// <para>���ê���Ƿ���ȷ��</para>
        /// </summary>
        private void CheckAnchor(RectTransform rt)
        {
            if (direction == AxisType.Vertical)
            {
                if (!((rt.anchorMin == new Vector2(0, 1) && rt.anchorMax == new Vector2(0, 1)) ||
                      (rt.anchorMin == new Vector2(0, 1) && rt.anchorMax == new Vector2(1, 1))))
                {
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(1, 1);
                }
            }
            else
            {
                if (!((rt.anchorMin == new Vector2(0, 1) && rt.anchorMax == new Vector2(0, 1)) ||
                      (rt.anchorMin == new Vector2(0, 0) && rt.anchorMax == new Vector2(0, 1))))
                {
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(0, 1);
                }
            }
        }

        /// <summary>
        /// When the scroll update the view.
        /// <para>������ʱ������ͼ</para>
        /// </summary>
        private void UpdateCheck(Vector2 v2)
        {
            if (m_ElementInfosArray == null) return;

            int _count = m_ElementInfosArray.Length;
            for (int i = 0, length = _count; i < length; i++)
            {
                ElementInfo _element = m_ElementInfosArray[i];
                GameObject obj = _element.Element;
                Vector3 pos = _element.Postation;
                float rangePos = direction == AxisType.Vertical ? pos.y : pos.x;

                if (IsOutRange(rangePos))
                {
                    if (obj != null)
                    {
                        SetPoolsObj(obj);
                        m_ElementInfosArray[i].Element = null;
                    }
                }
                else
                {
                    if (obj == null)
                    {
                        GameObject cell = GetPoolsObj();
                        cell.transform.localPosition = pos;
                        //cell.name = i.ToString();
                        m_ElementInfosArray[i].Element = cell;
                        CallbackFunction(cell, i);
                    }
                }
            }
        }

        /// <summary>
        /// Check whether it is out of the display range
        /// <para>����Ƿ񳬳���ʾ��Χ</para>
        /// </summary>
        /// <param name="pos">The element position. <para>Ԫ��λ��</para></param>
        protected bool IsOutRange(float pos)
        {
            Vector3 listP = content.anchoredPosition;
            if (direction == AxisType.Vertical)
            {
                if (pos + listP.y > m_ElementHeight || pos + listP.y < -m_Rect.rect.height)
                {
                    return true;
                }
            }
            else
            {
                if (pos + listP.x < -m_ElementWidth || pos + listP.x > m_Rect.rect.width)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Show the list with new count.
        /// <para>�������ݸ����б�</para>
        /// </summary>
        void ShowList(int num)
        {
            m_MinIndex = -1;
            m_MaxIndex = -1;

            if (direction == AxisType.Vertical)
            {
                float contentSize = (m_Spacing.y + m_ElementHeight) * Mathf.CeilToInt((float)num / m_Lines);
                m_ContentHeight = contentSize;
                m_ContentWidth = content.sizeDelta.x;
                contentSize = contentSize < m_Rect.rect.height ? m_Rect.rect.height : contentSize;
                content.sizeDelta = new Vector2(m_ContentWidth, contentSize);
                if (num != m_MaxElementalCount)
                {
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
                }
            }
            else
            {
                float contentSize = (m_Spacing.x + m_ElementWidth) * Mathf.CeilToInt((float)num / m_Lines);
                m_ContentWidth = contentSize;
                m_ContentHeight = content.sizeDelta.x;
                contentSize = contentSize < m_Rect.rect.width ? m_Rect.rect.width : contentSize;
                content.sizeDelta = new Vector2(contentSize, m_ContentHeight);
                if (num != m_MaxElementalCount)
                {
                    content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
                }
            }

            int lastEndIndex = 0;

            if (m_Inited)
            {
                lastEndIndex = num - m_MaxElementalCount > 0 ? m_MaxElementalCount : num;
                //lastEndIndex = m_ClearList ? 0 : lastEndIndex;

                int count = m_MaxElementalCount;
                for (int i = lastEndIndex; i < count; i++)
                {
                    if (m_ElementInfosArray[i].Element != null)
                    {
                        SetPoolsObj(m_ElementInfosArray[i].Element);
                        m_ElementInfosArray[i].Element = null;
                    }
                }
            }

            ElementInfo[] _tempCellInfos = m_ElementInfosArray;
            m_ElementInfosArray = new ElementInfo[num];

            for (int i = 0; i < num; i++)
            {
                if (m_MaxElementalCount != -1 && i < lastEndIndex)
                {
                    ElementInfo _ei = _tempCellInfos.Length > i ? _tempCellInfos[i] : new ElementInfo();

                    float rPos = direction == AxisType.Vertical ? _ei.Postation.y : _ei.Postation.x;
                    if (!IsOutRange(rPos))
                    {
                        m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;
                        m_MaxIndex = i;

                        if (_ei.Element == null)
                        {
                            _ei.Element = GetPoolsObj();
                        }

                        _ei.Element.transform.GetComponent<RectTransform>().localPosition = _ei.Postation;
                        _ei.Element.name = i.ToString();
                        _ei.Element.SetActive(true);

                        CallbackFunction(_ei.Element, i);
                    }
                    else
                    {
                        SetPoolsObj(_ei.Element);
                        _ei.Element = null;
                    }

                    m_ElementInfosArray[i] = _ei;
                    continue;
                }

                ElementInfo _element = new ElementInfo();

                if (direction == AxisType.Vertical)
                {
                    _element.Postation = new Vector3(m_ElementWidth * (i % m_Lines) + m_Spacing.x * (i % m_Lines), -(m_ElementHeight * Mathf.FloorToInt(i / m_Lines) + m_Spacing.y * Mathf.FloorToInt(i / m_Lines)), 0);
                }
                else
                {
                    _element.Postation = new Vector3(m_ElementWidth * Mathf.FloorToInt(i / m_Lines) + m_Spacing.x * Mathf.FloorToInt(i / m_Lines), -(m_ElementHeight * (i % m_Lines) + m_Spacing.y * (i % m_Lines)), 0);
                }

                float cellPos = direction == AxisType.Vertical ? _element.Postation.y : _element.Postation.x;
                if (IsOutRange(cellPos))
                {
                    _element.Element = null;
                    m_ElementInfosArray[i] = _element;
                    continue;
                }

                m_MinIndex = m_MinIndex == -1 ? i : m_MinIndex;
                m_MaxIndex = i;

                GameObject cell = GetPoolsObj();
                cell.GetComponent<RectTransform>().localPosition = _element.Postation;
                //cell.name = i.ToString();

                _element.Element = cell;
                m_ElementInfosArray[i] = _element;

                CallbackFunction(cell, i);
            }
            m_MaxCount = num;
            m_MaxElementalCount = num;
            m_Inited = true;
        }

        /// <summary>
        /// The callback event with elements list update.
        /// <para>����Ԫ���б�Ļص��¼���</para>
        /// </summary>
        protected void CallbackFunction(GameObject obj, int index)
        {
            CallbackFunc?.Invoke(obj, index);
            m_CurrentIndex = index;
        }

        #endregion

        /// <summary>
        /// Initialize the scroll view pro.
        /// <para>��ʼ��������ͼ</para>
        /// </summary>
        /// <param name="callback">The update event. <para>���»ص�����</para></param>
        /// <param name="maxCount">The elements list max count. <para>���Ԫ������</para></param>
        public void InIt(Action<GameObject, int> callback, int maxCount)
        {
            CallbackFunc = null;
            CallbackFunc = callback;

            if (m_Inited) return;

            m_ElementsPool = new Stack<GameObject>();
            SetPoolsObj(Elemental);

            RectTransform cellRectTrans = Elemental.GetComponent<RectTransform>();
            cellRectTrans.pivot = new Vector2(0f, 1f);
            cellRectTrans.anchorMax = new Vector2(0f, 1f);
            Vector2 _v2 = cellRectTrans.sizeDelta;
            if (_v2.x == 0)
                cellRectTrans.sizeDelta = new Vector2(Screen.width, _v2.y);
            if (_v2.y == 0)
                cellRectTrans.sizeDelta = new Vector2(_v2.x, Screen.height);
            CheckAnchor(cellRectTrans);
            cellRectTrans.anchoredPosition = Vector2.zero;

            m_ElementHeight = cellRectTrans.rect.height;
            m_ElementWidth = cellRectTrans.rect.width;
            content.pivot = new Vector2(0f, 1f);
            m_ContentHeight = content.rect.height;
            m_ContentWidth = content.rect.width;

            CheckAnchor(content);
            m_Inited = true;
            ShowList(maxCount);
        }

        /// <summary>
        /// Changed the lines of Scroll view pro.
        /// <para>������/����</para>
        /// </summary>
        /// <param name="maxCount">Max count. <para>�����/����</para></param>
        public void ChangedLinesNumber(int maxCount)
        {
            if (!m_Inited) return;
            ShowList(maxCount);
        }

        /// <summary>
        /// restore to the original position with index 0
        /// <para>�ָ�������Ϊ0��ԭʼλ��</para>
        /// </summary>
        public void GoToStartLine()
        {
            GoToElementPosWithIndex(0);
        }

        /// <summary>
        /// Go to position with element index.
        /// <para>ͨ��������λ��ĳһ��Ԫ�������λ��</para>
        /// </summary>
        /// <param name="index">The element index. ����ID</param>
        public void GoToElementPosWithIndex(int index)
        {
            if (null == m_ElementInfosArray || m_ElementInfosArray.Length == 0) return;

            int theFirstIndex = index - index % m_Lines;
            var tmpIndex = theFirstIndex + m_MaxIndex;

            int theLastIndex = tmpIndex > m_MaxElementalCount - 1 ? m_MaxElementalCount - 1 : tmpIndex;

            if (theLastIndex == m_MaxElementalCount - 1)
            {
                var shortOfNum = m_MaxElementalCount % m_Lines == 0 ? 0 : m_Lines - m_MaxElementalCount % m_Lines;
                theFirstIndex = theLastIndex - m_MaxIndex + shortOfNum;
            }

            Vector2 newPos = m_ElementInfosArray[theFirstIndex].Postation;
            if (direction == AxisType.Vertical)
            {
                var posY = index <= m_Lines ? -newPos.y : -newPos.y - m_Spacing.y;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, posY);
            }
            else
            {
                var posX = index <= m_Lines ? -newPos.x : -newPos.x + m_Spacing.x;
                content.anchoredPosition = new Vector2(posX, content.anchoredPosition.y);
            }
        }
    }
}
