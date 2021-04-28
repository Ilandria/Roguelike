using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Character Management Controller", menuName = "CCB/Controller/Character Management Controller")]
	public class CharacterManagementController : ScriptableObject
	{
		[SerializeField]
		private CharacterDataRepository characterDataRepository = null;

		public IEnumerable<CharacterDataModel> Characters => characterDataRepository.DataModels;

		public CharacterDataModel ActiveCharacter { get; private set; }

		public event CharacterDataModelDelegate OnNewCharacterCreated;

		public event CharacterDataModelDelegate OnCreateNewCharacterFailed;

		public event GuidDelegate OnCharacterDeleteRequested;

		public event GuidDelegate OnCharacterDeleted;

		public event GuidDelegate OnCharacterDeleteFailed;

		public event GuidDelegate OnCharacterSaved;

		public event GuidDelegate OnCharacterSaveFailed;

		public event Delegate OnStartSinglePlayer;

		public void CreateNewCharacter()
		{
			CharacterDataModel characterData = characterDataRepository.CreateNewCharacter();

			if (characterData != null)
			{
				OnNewCharacterCreated?.Invoke(characterData);
			}
			else
			{
				OnCreateNewCharacterFailed?.Invoke(characterData);
			}
		}

		public void RequestCharacterDeletion(Guid characterGuid)
		{
			OnCharacterDeleteRequested?.Invoke(characterGuid);
		}

		public void DeleteCharacter(Guid characterGuid)
		{
			if (characterDataRepository.DeleteCharacter(characterGuid))
			{
				OnCharacterDeleted?.Invoke(characterGuid);
			}
			else
			{
				OnCharacterDeleteFailed?.Invoke(characterGuid);
			}
		}

		public void SaveCharacter(Guid characterGuid)
		{
			if (characterDataRepository.SaveCharacter(characterGuid))
			{
				OnCharacterSaved?.Invoke(characterGuid);
			}
			else
			{
				OnCharacterSaveFailed?.Invoke(characterGuid);
			}
		}

		public void ClearActiveCharacter()
		{
			ActiveCharacter = null;
		}

		// Todo: This doesn't really belong in this class, it should eventually be moved out.
		public void StartSinglePlayer(CharacterDataModel characterData)
		{
			if (ActiveCharacter == null)
			{
				ActiveCharacter = characterData;
				OnStartSinglePlayer?.Invoke();
			}
		}
	}
}
