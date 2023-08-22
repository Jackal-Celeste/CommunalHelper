local drawableRectangle = require("structs.drawable_rectangle")
local fakeTilesHelper = require("helpers.fake_tiles")

local chainedPendulumBlock = {}

chainedPendulumBlock.name = "CommunalHelper/ChainedPendulumBlock"
chainedPendulumBlock.fieldInformation = {
    tiletype = {
        options = fakeTilesHelper.getTilesOptions(),
        editable = false
    },
    fallDistance = {
        minimumValue = 0,
        fieldType = "integer"
    }
}

function chainedPendulumBlock.depth(room, entity)
    return entity.behind and 5000 or 0
end

chainedPendulumBlock.placements = {
    name = "chained_pendulum_block",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        climbFall = true,
        behind = false,
        fallDistance = 64,
        centeredChain = false,
        chainOutline = true,
        indicator = false,
        indicatorAtStart = false,
        frequency = "4",
        maxAngle = "90",
        decayRate = "30",
        stickOnDash = false

    }
}

local fakeTilesSpriteFunction = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)

function chainedPendulumBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8

    local sprites = fakeTilesSpriteFunction(room, entity)

    local fallDistance = entity.fallDistance or 16
    local rect = drawableRectangle.fromRectangle("line", x, y, width, height + fallDistance, {1, 1, 1, 0.5})
    rect.depth = 0
    table.insert(sprites, rect)

    return sprites
end

return chainedPendulumBlock
