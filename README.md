<body lang=PT-BR style='tab-interval:35.4pt'>

<div class=WordSection1>

<p class=MsoNormal align=center style='text-align:center'><b style='mso-bidi-font-weight:
normal'><u><span style='font-size:14.0pt;mso-bidi-font-size:11.0pt;line-height:
107%'>Roteiro para execução do Projeto<o:p></o:p></span></u></b></p>

<p class=MsoNormal align=center style='text-align:center'><b style='mso-bidi-font-weight:
normal'><u><span style='font-size:14.0pt;mso-bidi-font-size:11.0pt;line-height:
107%'><o:p><span style='text-decoration:none'>&nbsp;</span></o:p></span></u></b></p>

<p class=MsoListParagraph style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span
style='mso-bidi-font-family:Calibri;mso-bidi-theme-font:minor-latin'><span
style='mso-list:Ignore'>1.<span style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><![endif]>Execute o arquivo <span class=SpellE>SCRIPT_LTM.sql</span>
no banco de dados SQL Server.</p>

<p class=MsoNormal style='margin-left:36.0pt'>O mesmo irá criar o Banco Test,
as tabelas e inserir os dados necessários para execução do projeto. </p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span
style='mso-bidi-font-family:Calibri;mso-bidi-theme-font:minor-latin'><span
style='mso-list:Ignore'>2.<span style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><![endif]>Abra o arquivo <span class=SpellE><b
style='mso-bidi-font-weight:normal'><span style='color:#00B050'>Web.config</span></b></span><span
style='color:#00B050'> </span>do projeto <span class=SpellE><span class=GramE><b
style='mso-bidi-font-weight:normal'><span style='color:#00B050'>LTM.Test.Web</span></b></span></span><b
style='mso-bidi-font-weight:normal'><span style='color:#00B050'> </span></b>(<span
class=SpellE>LTM.Test.Web</span>\<span class=SpellE>Web.config</span>) para
edição.</p>

<p class=MsoListParagraphCxSpMiddle><o:p>&nbsp;</o:p></p>

<p class=MsoListParagraphCxSpLast style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span
style='mso-bidi-font-family:Calibri;mso-bidi-theme-font:minor-latin'><span
style='mso-list:Ignore'>3.<span style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><![endif]>Alterar a <span class=SpellE>ConnectionString</span>
na linha 10 como segue.</p>

<p class=MsoNormal><span lang=EN-US style='font-size:9.5pt;line-height:107%;
font-family:Consolas;color:blue;mso-ansi-language:EN-US'>&lt;</span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:#A31515;mso-ansi-language:EN-US'>add</span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'> </span><span lang=EN-US style='font-size:9.5pt;
line-height:107%;font-family:Consolas;color:red;mso-ansi-language:EN-US'>name</span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:blue;mso-ansi-language:EN-US'>=</span><span lang=EN-US style='font-size:
9.5pt;line-height:107%;font-family:Consolas;color:black;mso-ansi-language:EN-US'>&quot;</span><span
class=SpellE><span class=GramE><span lang=EN-US style='font-size:9.5pt;
line-height:107%;font-family:Consolas;color:blue;mso-ansi-language:EN-US'>LTM.DBAccess.ConnectionString</span></span></span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:black;mso-ansi-language:EN-US'>&quot;</span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'> </span><span class=SpellE><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:red;
mso-ansi-language:EN-US'>providerName</span></span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'>=</span><span lang=EN-US style='font-size:9.5pt;
line-height:107%;font-family:Consolas;color:black;mso-ansi-language:EN-US'>&quot;</span><span
class=SpellE><span lang=EN-US style='font-size:9.5pt;line-height:107%;
font-family:Consolas;color:blue;mso-ansi-language:EN-US'>System.Data.SqlClient</span></span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:black;mso-ansi-language:EN-US'>&quot;</span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'> </span><span class=SpellE><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:red;
mso-ansi-language:EN-US'>connectionString</span></span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'>=</span><span lang=EN-US style='font-size:9.5pt;
line-height:107%;font-family:Consolas;color:black;mso-ansi-language:EN-US'>&quot;</span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:blue;mso-ansi-language:EN-US'>Server=<span style='background:yellow;
mso-highlight:yellow'>[SERVIDOR]</span>;Database=<span class=SpellE>Test;User</span>
Id=<span style='background:yellow;mso-highlight:yellow'>[USUARIO]</span>;Password=<span
style='background:yellow;mso-highlight:yellow'>[SENHA]</span>;</span><span
lang=EN-US style='font-size:9.5pt;line-height:107%;font-family:Consolas;
color:black;mso-ansi-language:EN-US'>&quot;</span><span lang=EN-US
style='font-size:9.5pt;line-height:107%;font-family:Consolas;color:blue;
mso-ansi-language:EN-US'> /&gt;<o:p></o:p></span></p>

<p class=MsoNormal><span lang=EN-US style='font-size:9.5pt;line-height:107%;
font-family:Consolas;color:blue;mso-ansi-language:EN-US'><span
style='mso-tab-count:1'>       </span></span>Alterar os campos [SERVIDOR], [USUARIO]
e [SENHA]</p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span
style='mso-bidi-font-family:Calibri;mso-bidi-theme-font:minor-latin'><span
style='mso-list:Ignore'>4.<span style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><![endif]>Abra o projeto <b style='mso-bidi-font-weight:
normal'><span style='color:#00B050'>LTM.sln</span></b> no Microsoft Visual
Studio <span class=SpellE>Community</span> 2017.</p>

<p class=MsoListParagraphCxSpMiddle><o:p>&nbsp;</o:p></p>

<p class=MsoListParagraphCxSpLast style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span
style='mso-bidi-font-family:Calibri;mso-bidi-theme-font:minor-latin'><span
style='mso-list:Ignore'>5.<span style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><![endif]>Os projetos <span class=SpellE><b
style='mso-bidi-font-weight:normal'><span style='color:#00B050'>LTM.Test.API</span></b></span>
e <span class=SpellE><span class=GramE><b style='mso-bidi-font-weight:normal'><span
style='color:#00B050'>LTM.Test.Web</span></b></span></span> devem ser
executados juntos como segue imagem.</p>

<p class=MsoNormal><span style='mso-fareast-language:PT-BR;mso-no-proof:yes'><!--[if gte vml 1]><v:shapetype
 id="_x0000_t75" coordsize="21600,21600" o:spt="75" o:preferrelative="t"
 path="m@4@5l@4@11@9@11@9@5xe" filled="f" stroked="f">
 <v:stroke joinstyle="miter"/>
 <v:formulas>
  <v:f eqn="if lineDrawn pixelLineWidth 0"/>
  <v:f eqn="sum @0 1 0"/>
  <v:f eqn="sum 0 0 @1"/>
  <v:f eqn="prod @2 1 2"/>
  <v:f eqn="prod @3 21600 pixelWidth"/>
  <v:f eqn="prod @3 21600 pixelHeight"/>
  <v:f eqn="sum @0 0 1"/>
  <v:f eqn="prod @6 1 2"/>
  <v:f eqn="prod @7 21600 pixelWidth"/>
  <v:f eqn="sum @8 21600 0"/>
  <v:f eqn="prod @7 21600 pixelHeight"/>
  <v:f eqn="sum @10 21600 0"/>
 </v:formulas>
 <v:path o:extrusionok="f" gradientshapeok="t" o:connecttype="rect"/>
 <o:lock v:ext="edit" aspectratio="t"/>
</v:shapetype><v:shape id="Imagem_x0020_1" o:spid="_x0000_i1025" type="#_x0000_t75"
 style='width:424.5pt;height:267pt;visibility:visible;mso-wrap-style:square'>
 <v:imagedata src="Roteiro_arquivos/image001.png" o:title=""/>
</v:shape><![endif]--><![if !vml]><img width=566 height=356
src="https://user-images.githubusercontent.com/868341/30758666-b05bab3e-9fa9-11e7-9e1a-5925a7ca3b28.png" v:shapes="Imagem_x0020_1"><![endif]></span></p>

<p class=MsoNormal><o:p>&nbsp;</o:p></p>

<p class=MsoNormal><b style='mso-bidi-font-weight:normal'>Usuário e senha para
teste:<o:p></o:p></b></p>

<p class=MsoNormal>Usuário: <span class=SpellE><span style='color:#C00000'>filipemiranda</span></span><span
style='color:#C00000'><br>
</span>Senha: <span class=SpellE><span style='color:#C00000'>filipemiranda</span></span></p>

</div>

</body>

