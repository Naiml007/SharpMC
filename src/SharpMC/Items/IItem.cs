﻿namespace SharpMC.Items
{
    public interface IItem
    {
        bool IsUsable { get; }

        int Id { get; }
    }
}