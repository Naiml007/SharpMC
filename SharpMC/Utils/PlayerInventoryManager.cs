﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpMC.Entity;
using SharpMC.Networking.Packages;

namespace SharpMC.Utils
{
	public class PlayerInventoryManager
	{
		private readonly Player _player;
		private readonly List<ItemStack> _slots = new List<ItemStack>();
		public ItemStack ClickedItem { get; set; }
		public int CurrentSlot { get; set; }

		public PlayerInventoryManager(Player player)
		{
			_player = player;
			for (var i = 0; i <= 44; i++)
			{
				_slots.Add(new ItemStack(-1, 0, 0));
			}
			
			SetSlot(5, 310, 0, 1); //Diamond helmet
			SetSlot(6, 311, 0, 1); //Diamond chestplate
			SetSlot(7, 312, 0, 1); //Diamond leggings
			SetSlot(8, 313, 0, 1); //Diamond boots

			SetSlot(36, 276, 0, 1); //Diamond sword
			SetSlot(37, 277, 0, 1); //Diamond shovel
			SetSlot(38, 278, 0, 1); //Diamond pickaxe
			SetSlot(39, 279, 0, 1); //Diamond axe

			SetSlot(43, 5, 2, 64);
			SetSlot(44, 20, 0, 12);

			SetSlot(41, 327, 0, 1);
			SetSlot(42, 326, 0, 1);
			SetSlot(40, 325, 0, 1);
		}

		public void SetSlot(int slot, short itemId, byte metadata, byte itemcount)
		{
			if (slot <= 44 && slot > 0)
			{
				_slots[slot] = new ItemStack(itemId, itemcount, metadata);
				if (_player != null && _player.IsSpawned)
				{
					new SetSlot(_player.Wrapper)
					{
						WindowId = 0,
						ItemId = itemId,
						ItemCount = itemcount,
						MetaData = metadata,
						ItemDamage = 0,
						Slot = (short) slot
					}.Write();
				}
			}
		}

		public bool AddItem(ItemStack item)
		{
			return AddItem(item.ItemId, item.MetaData, item.ItemCount);
		}

		public bool AddItem(short itemId, byte metadata, byte itemcount = 1)
		{

			for (var i = 9; i <= 44; i++)
			{
				if (_slots[i].ItemId == itemId && _slots[i].MetaData == metadata && _slots[i].ItemCount < 64)
				{
					var oldslot = _slots[i];
					if (oldslot.ItemCount + itemcount <= 64)
					{
						SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemcount));
						return true;
					}
					else
					{
						SetSlot(i, itemId, metadata, 64);
						int remaining = (oldslot.ItemCount + itemcount) - 64;
						return AddItem(itemId, metadata, (byte) remaining);
					}
				}
			}

			for (var i = 9; i <= 44; i++)
			{
				if (_slots[i].ItemId == -1)
				{
					SetSlot(i, itemId, metadata, itemcount);
					return true;
				}
			}
			return false;
		}

		public ItemStack GetSlot(int slot)
		{
			if (slot <= 44 && slot > 0)
			{
				return _slots[slot];
			}
			throw new IndexOutOfRangeException("slot");
		}

		public void DropCurrentItem()
		{
			//Drop the current hold item
			int slottarget = 36 + CurrentSlot;
			var slot = GetSlot(slottarget);
			if (slot.ItemCount > 1)
			{
				SetSlot(slottarget, slot.ItemId, slot.MetaData, (byte) (slot.ItemCount -1));
			}
			else
			{
				SetSlot(slottarget, -1, 0, 0);
			}

			if (slot.ItemId != -1)
			{
				new ItemEntity(_player.Level, new ItemStack(slot.ItemId, 1, slot.MetaData)) {KnownPosition = _player.KnownPosition}
					.SpawnEntity();
			}
		}

		public void DropCurrentItemStack()
		{
			//Drop current hold item stack
			ConsoleFunctions.WriteDebugLine("Unimplemented feature called!");
		}

		public bool HasItem(int itemId)
		{
			if (_slots.Any(itemStack => itemStack.ItemId == itemId))
			{
				return true;
			}
			return false;
		}

		public bool RemoveItem(short itemId, short metaData, short count)
		{
			for (int index = 0; index < _slots.Count; index++)
			{
				ItemStack itemStack = _slots[index];
				if (itemStack.ItemId == itemId && itemStack.MetaData == metaData && itemStack.ItemCount >= count)
				{
					if ((itemStack.ItemCount - count) > 0)
					{
						SetSlot(index, itemStack.ItemId, itemStack.MetaData, (byte) (itemStack.ItemCount - count));
						return true;
					}
					else
					{
						SetSlot(index, -1, 0, 0);
						return true;
					}
				}
			}
			return false;
		}

		public void SendToPlayer()
		{
			for (short i = 0; i <= 44; i++)
			{
				var value = _slots[i];
				if (value.ItemId != -1)
				{
					new SetSlot(_player.Wrapper)
					{
						WindowId = 0,
						ItemId = value.ItemId,
						ItemCount = value.ItemCount,
						MetaData = value.MetaData,
						Slot = i
					}.Write();
				}
			}
		}
	}
}