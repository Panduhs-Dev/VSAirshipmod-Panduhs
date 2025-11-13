using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;
using VSAirshipmod.NSBlockEntity;


namespace VSAirshipmod.NSBlock
{
    public class BlockSpinner : BlockMPBase
    {
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            bool ok = base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
            if (!ok) return false;

            BlockPos pos = blockSel.Position;
            Block block = world.BlockAccessor.GetBlock(pos);

            string facing = block.LastCodePart(); // checking via code path this time unlike autoloom
            BlockFacing backFace = BlockFacing.FromCode(facing);

            tryConnect(world, byPlayer, pos, backFace);
            return true;
        }





        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel == null || !world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
                return false;

            var be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntitySpinner;
            if (be == null) return false;

            // Let the BE handle GUI/client logic
            if (be.OnPlayerRightClick(byPlayer, blockSel))
                return true;

            return base.OnBlockInteractStart(world, byPlayer, blockSel);
        }



        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction
                {
                    ActionLangCode = "blockhelp-quern-addremoveitems",
                    MouseButton = EnumMouseButton.Right
                }
            }.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
        }

        public override bool HasMechPowerConnectorAt(IWorldAccessor world, BlockPos pos, BlockFacing face)
        {
            Block block = world.BlockAccessor.GetBlock(pos);

            string facing = block.LastCodePart();
            BlockFacing backFace = BlockFacing.FromCode(facing);
            return face == backFace;
        }



        public override void DidConnectAt(IWorldAccessor world, BlockPos pos, BlockFacing face)
        {

        }
    }
}