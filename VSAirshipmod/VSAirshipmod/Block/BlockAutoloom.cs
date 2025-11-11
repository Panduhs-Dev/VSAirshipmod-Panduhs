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
    public class BlockAutoloom : BlockMPBase
    {
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            bool ok = base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
            if (!ok) return false;

            BlockPos pos = blockSel.Position;
            Block block = world.BlockAccessor.GetBlock(pos);

            float rotY = block.Shape?.rotateY ?? 0f;
            int rotSteps = ((int)Math.Round(rotY / 90f) % 4 + 4) % 4;

            BlockFacing[] horizontals = BlockFacing.HORIZONTALS;
            int index = Array.IndexOf(horizontals, BlockFacing.WEST);
            int rotatedIndex = (index + rotSteps) % 4;

            if (rotSteps == 1 || rotSteps == 3)
            {
                rotatedIndex = (rotatedIndex + 2) % 4;
            }

            BlockFacing connectorFace = horizontals[rotatedIndex];

            tryConnect(world, byPlayer, pos, connectorFace);

            return true;
        }




        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel == null || !world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
                return false;

            var be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntityAutoloom;
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

            float rotY = block.Shape?.rotateY ?? 0f;
            int rotSteps = ((int)Math.Round(rotY / 90f) % 4 + 4) % 4;
            BlockFacing[] horizontals = BlockFacing.HORIZONTALS;
            int index = Array.IndexOf(horizontals, BlockFacing.WEST);
            int rotatedIndex = (index + rotSteps) % 4;

            // Flip for east/west rotations
            if (rotSteps == 1 || rotSteps == 3)
            {
                rotatedIndex = (rotatedIndex + 2) % 4;
            }

            BlockFacing connector = horizontals[rotatedIndex];

            return face == connector;
        }


        public override void DidConnectAt(IWorldAccessor world, BlockPos pos, BlockFacing face)
        {

        }
    }
}