using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Sprite Builder Definition", menuName = "CCB/Data/Sprite Builder Definition")]
	public class SpriteSheetDescription : ScriptableObject
	{
		[SerializeField]
		private Vector2Int spriteSize = Vector2Int.zero;
		public Vector2Int SpriteSize => spriteSize;

		[SerializeField]
		private Vector2 spritePivot = Vector2.zero;
		public Vector2 SpritePivot => spritePivot;

		[SerializeField]
		private List<AnimationInfo> animations = new List<AnimationInfo>();
		public List<AnimationInfo> Animations => animations;
	}
}