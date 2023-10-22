﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
	public class SelectionHandler : CallbackHandler
	{
		public override string[] Callbacks
		{
			get
			{
				return new[] {
					"OnSelect",
					"OnDeselect",
				};
			}
		}

		[EnumFlags]
		public SelectionInputType selectionType = SelectionInputType.LeftClick | SelectionInputType.Raycast;
		[SerializeField]
		private float m_SelectionDistance = 10f;
		[InspectorLabel("Key")]
		[SerializeField]
		private KeyCode m_SelectionKey = KeyCode.F;
		[SerializeField]
		private Vector3 m_RaycastOffset = Vector3.zero;
		[SerializeField]
		private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

		[EnumFlags]
		public DeselectionInputType deselectionType = DeselectionInputType.LeftClick | DeselectionInputType.Key;
		[InspectorLabel("Key")]
		[SerializeField]
		private KeyCode m_DeselectionKey = KeyCode.Escape;
		[InspectorLabel("Distance")]
		[SerializeField]
		private float m_DeselectionDistance = 20f;

		private readonly float m_UpdateInterval = 1f;
		private Camera m_Camera;
		private Transform m_CameraTransform;
		private Transform m_Transform;
		private Vector3 m_Offset;
		private ISelectable m_CurrentSelection;

		private void Start()
		{
			m_Camera = Camera.main;
			m_CameraTransform = m_Camera.transform;
			m_Transform = transform;
			InvokeRepeating("CustomUpdate", 1f, m_UpdateInterval);
		}

		private void CustomUpdate()
		{
			if (m_CurrentSelection is not null && deselectionType.HasFlag<DeselectionInputType>(DeselectionInputType.Distance) && Vector3.Distance(m_Transform.position, m_CurrentSelection.position) > m_DeselectionDistance)
			{
				Deselect();
			}
		}

		private void Update()
		{
			bool selected = false;
			//Selection
			if ((Input.GetMouseButtonDown(0) && selectionType.HasFlag<SelectionInputType>(SelectionInputType.LeftClick)) ||
				(Input.GetMouseButtonDown(1) && selectionType.HasFlag<SelectionInputType>(SelectionInputType.RightClick)) ||
				(Input.GetMouseButtonDown(2) && selectionType.HasFlag<SelectionInputType>(SelectionInputType.MiddleClick)))
			{
				selected = TrySelect(m_Camera.ScreenPointToRay(Input.mousePosition));
			}
			else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && selectionType.HasFlag<SelectionInputType>(SelectionInputType.Raycast))
			{
				selected = TrySelect(new Ray(m_CameraTransform.position, m_CameraTransform.forward + m_RaycastOffset));
			}
			else if (Input.GetKeyDown(m_SelectionKey) && selectionType.HasFlag<SelectionInputType>(SelectionInputType.Key))
			{
				RaycastHit[] hits = Physics.SphereCastAll(m_Transform.position, m_SelectionDistance, m_Transform.forward);
				ISelectable selectable = GetBestSelectable(hits.Select(x => x.collider.GetComponent<ISelectable>()).OfType<ISelectable>().Where(x => x.enabled));
				if (selectable != null)
				{
					Select(selectable);
					selected = true;
				}
			}

			//Deselection
			if (!UnityTools.IsPointerOverUI())
			{
				if (!selected && ((Input.GetMouseButtonDown(0) && deselectionType.HasFlag<DeselectionInputType>(DeselectionInputType.LeftClick)) ||
					(Input.GetMouseButtonDown(1) && deselectionType.HasFlag<DeselectionInputType>(DeselectionInputType.RightClick)) ||
					(Input.GetMouseButtonDown(2) && deselectionType.HasFlag<DeselectionInputType>(DeselectionInputType.MiddleClick)) ||
					(Input.GetKeyDown(m_DeselectionKey) && deselectionType.HasFlag<DeselectionInputType>(DeselectionInputType.Key))))
				{
					Deselect();
				}
			}
		}

		private bool TrySelect(Ray ray)
		{

			if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, m_LayerMask))
			{
				if (Vector3.Distance(m_Transform.position, hit.transform.position) < m_SelectionDistance)
				{
					ISelectable selectable = hit.collider.GetComponent<ISelectable>();
					if (selectable != null && selectable.enabled)
					{
						Select(selectable);
						return true;
					}
				}
			}

			return false;
		}

		public void Select(ISelectable selectable)
		{
			Deselect();
			m_CurrentSelection = selectable;
			m_CurrentSelection.OnSelect();
		}

		public void Deselect()
		{
			if (m_CurrentSelection != null)
			{
				m_CurrentSelection.OnDeselect();
				m_CurrentSelection = null;
			}
		}

		private ISelectable GetBestSelectable(IEnumerable<ISelectable> selectables)
		{
			ISelectable tMin = null;
			float minDist = Mathf.Infinity;
			Vector3 currentPos = m_Transform.position;
			foreach (ISelectable selectable in selectables)
			{

				Vector3 dir = selectable.position - currentPos;

				float dist = Vector3.Distance(selectable.position, currentPos) * Quaternion.Angle(m_Transform.rotation, Quaternion.LookRotation(dir));
				if (dist < minDist)
				{
					tMin = selectable;
					minDist = dist;
				}
			}

			return tMin;
		}

		[System.Flags]
		public enum SelectionInputType
		{
			LeftClick = 1,
			RightClick = 2,
			Key = 4,
			Raycast = 8,
			MiddleClick = 16
		}

		[System.Flags]
		public enum DeselectionInputType
		{
			LeftClick = 1,
			RightClick = 2,
			Key = 4,
			Distance = 8,
			MiddleClick = 16
		}
	}
}