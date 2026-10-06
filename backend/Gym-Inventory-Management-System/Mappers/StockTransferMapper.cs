using Gym_Inventory_Management_System.DTOs;
using Gym_Inventory_Management_System.Models;

namespace Gym_Inventory_Management_System.Mappers;
    public static class StockTransferMapper
    {
    public static StockTransferResponseDTO ToDTO(StockTransfer stockTransfer)
    {
        return new StockTransferResponseDTO(
            stockTransfer.Id,
            stockTransfer.ItemId,
            stockTransfer.FromLocationId,
            stockTransfer.ToLocationId,
            stockTransfer.Quantity,
            stockTransfer.TransferDate,
            stockTransfer.UserId
        );
    }

    public static StockTransfer ToEntity(this StockTransferCreateDTO dto)
    {
        return new StockTransfer
        {
            ItemId = dto.ItemId,
            FromLocationId = dto.FromLocationId,
            ToLocationId = dto.ToLocationId,
            Quantity = dto.Quantity,
            TransferDate = dto.TransferDate,
            UserId = dto.UserId
        };
    }
}

