if (isObject(VIR_HUD))
  VIR_HUD.delete();

if (!isObject(VIR_HUD))
{
  new GuiControl(VIR_HUD)
  {
    profile = GuiDefaultProfile;
    extent = PlayGUI.extent;
    horizSizing = "width";
    vertSizing = "height";
    // visible = false;

    new GuiSwatchCtrl()
    {
      profile = GuiDefaultProfile;
      position = 32 SPC getWord(PlayGUI.extent, 1) - 32 - 64;
      extent = "416 64";
      vertSizing = "top";
      color = "50 45 40 255";

      new GuiSwatchCtrl()
      {
        profile = GuiDefaultProfile;
        position = "8 8";
        extent = "400 20";
        color = "30 20 20 255";

        new GuiSwatchCtrl(VIR_HUD_Health_Slide)
        {
          profile = GuiDefaultProfile;
          position = "0 0";
          minExtent = "0 20";
          extent = "0 20";
          color = "160 0 80 255";
        };

        new GuiSwatchCtrl(VIR_HUD_Health_Fill)
        {
          profile = GuiDefaultProfile;
          position = "8 8";
          minExtent = "0 20";
          extent = "0 20";
          color = "255 40 30 255";
        };

        new GuiMLTextCtrl(VIR_HUD_Health_Text)
        {
          profile = BlockChatTextSize4Profile;
          position = "0 -1";
          extent = "400 20";
          text = "<just:center><font:palatino linotype:20><color:ffffff>0/0";
        };
      };

      new GuiSwatchCtrl()
      {
        profile = GuiDefaultProfile;
        position = "8 36";
        extent = "400 20";
        color = "20 20 30 255";

        new GuiSwatchCtrl(VIR_HUD_Mana_Fill)
        {
          profile = GuiDefaultProfile;
          position = "8 8";
          minExtent = "0 20";
          extent = "0 20";
          color = "50 140 255 255";
        };

        new GuiMLTextCtrl(VIR_HUD_Mana_Text)
        {
          profile = BlockChatTextSize4Profile;
          position = "0 -1";
          extent = "400 20";
          text = "<just:center><font:palatino linotype:20><color:ffffff>0/0";
        };
      };
    };
  };

  PlayGUI.add(VIR_HUD);

  VIR_SetHealth(100, 100);
  VIR_SetMana(100, 200);
}

function VIR_UpdateNotify(%notify, %fade)
{
  cancel(%notify.updateNotify);

  if (%fade >= 1)
    %notify.delete();

  if (!isObject(%notify))
    return;

  %res = getRes();

  %notify.setAlpha(1 - getMax(0, %fade * 2 - 1));
  %notify.resize(16, %notify.origin - %fade * %notify.movement, getWord(%res, 0) - 32, 0);
  %notify.setText(%notify.getText());

  %notify.updateNotify = schedule(1, 0, "VIR_UpdateNotify", %notify, %fade + 1 / %notify.duration);
}

function VIR_UpdateHealthSlide(%first)
{
  cancel($VIR_UpdateHealthSlide);

  %valueFill = VIR_HUD_Health_Fill.value;
  %value = VIR_HUD_Health_Slide.value;

  if (!%first)
    %value -= 0.00175;

  %value = getMax(0, getMin(1 - %valueFill, %value));

  VIR_HUD_Health_Slide.resize(%valueFill * 400, 0, %value * 400, 20);
  VIR_HUD_Health_Slide.value = %value;

  if (%value > 0)
    $VIR_UpdateHealthSlide = schedule(1, 0, "VIR_UpdateHealthSlide");
}

function VIR_ShowNotify(%text, %origin)
{
  if (%origin $= "")
    %origin = getWord(VIR_HUD.extent, 1) - 128;

  %notify = new GuiMLTextCtrl()
  {
    profile = BlockChatTextSize4Profile;
    text = %text;
    duration = 250; // 1000
    origin = %origin;
    movement = 128;
  };

  VIR_HUD.add(%notify);
  VIR_UpdateNotify(%notify, 0);
  return %notify;
}

function VIR_ShowXPNotify(%skill, %amount)
{
  %notify = new GuiMLTextCtrl()
  {
    profile = BlockChatTextSize4Profile;
    text = "<just:right><color:ffff00>" @ %skill @ " <color:ffffff>+" @ %amount @ " XP \n";
    duration = 300;
    origin = 108;
    movement = 100;
  };

  VIR_HUD.add(%notify);
  VIR_UpdateNotify(%notify, 0);
  return %notify;
}

function VIR_ShowItemNotify(%item, %amount)
{
  %notify = VIR_HUD.itemNotify[%item];

  if (!isObject(%notify))
  {
    %notify = new GuiMLTextCtrl()
    {
      profile = BlockChatTextSize4Profile;
      duration = 400;
      origin = getWord(VIR_HUD.extent, 1) - 128;
      movement = 128;
      amount = 0;
    };

    VIR_HUD.itemNotify[%item] = %notify;
    VIR_HUD.add(%notify);
  }

  %notify.amount = (%notify.amount + %amount) | 0;
  %notify.setText("<just:center><color:ffffff>+" @ %notify.amount @ " <color:ffff00>" @ %item.uiName);
  VIR_UpdateNotify(%notify, 0);
  return %notify;
}

function VIR_SetHealth(%cur, %max)
{
  %value = %cur / %max;

  if (%value < VIR_HUD_Health_Fill.value)
    VIR_HUD_Health_Slide.value = getMin(1 - %value, VIR_HUD_Health_Slide.value + (VIR_HUD_Health_Fill.value - %value));
  else if (%value > VIR_HUD_Health_Fill.value)
    VIR_HUD_Health_Slide.value = getMax(0, VIR_HUD_Health_Slide.value - (%value - VIR_HUD_Health_Fill.value));

  VIR_HUD_Health_Fill.value = %value;

  %wn = %value * 400;

  VIR_HUD_Health_Fill.resize(0, 0, %wn, 20);
  VIR_HUD_Health_Text.setText("<just:center><font:palatino linotype:20><color:ffffff>" @ %cur @ "/" @ %max);

  VIR_UpdateHealthSlide(true);
}

function VIR_SetMana(%cur, %max)
{
  %w = %cur / %max * 400;
  VIR_HUD_Mana_Fill.resize(0, 0, %w, 20);
  VIR_HUD_Mana_Text.setText("<just:center><font:palatino linotype:20><color:ffffff>" @ %cur @ "/" @ %max);
}
