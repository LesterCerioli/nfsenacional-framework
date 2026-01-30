# ns-nfsenacional-framework

Utilizando a NS API, este exemplo - criado em C# - possui funcionalidades para consumir o documento fiscal eletrônico: 
+ NFSe Nacional; 

Simplificando o projeto utilizado em um único exemplo, deixando mais pratica e facil a integração com a NS API.

## Primeiros passos:

### Integrando ao sistema:

Para utilizar as funções de comunicação com a API, você precisa realizar os seguintes passos:

1. Baixe a solução ns-nfsenacional-framework para sua maquina;
2. Copie para sua aplicação o projeto, no qual contem todos as classes que serão utilizadas;
3. Abra o seu projeto e importe a pasta copiada.
4.  A aplicação utiliza o ['Package Newtonsoft.Json'](https://docs.microsoft.com/pt-br/nuget/consume-packages/install-use-packages-visual-studio#finding-and-installing-a-package) para manipular os JSONs neste exemplo, respectivamente. 

**Pronto!** Agora, você já pode consumir a todos os projetos da NS API. Todas as funcionalidades de comunicação foram implementadas na classe NSNFSeNacional.cs.

------

## Emissão Sincrona:

### Realizando uma Emissão Sincrona:

Você poderá utilizar a função **emitirNFSeSincrono** da classe NSNFSeNacional. Veja abaixo sobre os parâmetros necessários, e um exemplo de chamada do método.

##### Parâmetros:

**ATENÇÃO:** o **token** também é um parâmetro necessário e você deve, primeiramente, defini-lo na classe **NSNFSeNacional.cs**.


Parametros     | Descrição
:-------------:|:-----------
**conteudo**   | Conteúdo de emissão do documento.
**tpConteudo** | Tipo de conteúdo que está sendo enviado. Valores possíveis: json, xml
**CNPJ**       | CNPJ do emitente do documento.
**tpDown**     | Tipo de arquivos a serem baixados.Valores possíveis: <ul> <li>**X** - XML</li> <li>**P** - PDF</li> <li>**XP** - XML e PDF</li> </ul> 
**tpAmb**      | Ambiente onde foi autorizado o documento.Valores possíveis:<ul> <li>1 - produção</li> <li>2 - homologação</li> </ul>
**caminho**    | Caminho onde devem ser salvos os documentos baixados.
**exibeNaTela**| Se for baixado a DFe, exibir o PDF na tela após a autorização. Valores possíveis: <ul> <li>**True** - será exibido</li> <li>**False** - não será exibido(Default)</li> </ul> 

##### Exemplo de chamada:

Após ter todos os parâmetros listados acima, você deverá fazer a chamada da função. Veja o código de exemplo abaixo:
           
    String retorno = NSNFSeNacional.emitirNFSeSincrono(conteudo, tpConteudo, cnpjEmit, tpDown, tpAmb, caminho, exibeNaTela)
    
    MessageBox.Show(retorno);

A função **emitirNFSeSincrono** fará o envio, a consulta e download do documento, utilizando as funções **emitirDocumento**, **consultarStatusProcessamento** e **downloadDocumentoESalvar**, presentes na classe NSNFSeNacional.cs. Tendo como retorno um JSON com os principais dados de emissão . No exemplo abaixo, veja como tratar o retorno da função emitirNFSeSincrono:

##### Exemplo de tratamento de retorno:

O JSON retornado pelo método terá os seguintes campos: statusEnvio, statusConsulta, statusDownload, cStat, chNFSe, chDPS, motivo, nsNRec, erros. Veja o exemplo abaixo:

    {
        "statusEnvio": "200",
        "statusConsulta": "200",
        "statusDownload": "200",
        "cStat": "100",
        "chNFSe": "31062002213278005000122000000000002926016153125604",
        "chDPS": "310620021327800500012200001000000000000029",
        "motivo": "Autorizado o uso da NFS-e",
        "nsNRec": "313022",
        "erros": ""
    }
      
Confira um código para tratamento do retorno, no qual pegará as informações dispostas no JSON de Retorno disponibilizado:

        String retorno = NSSuite.emitirNFSeSincrono(conteudo, tpConteudo, cnpjEmit, tpDown, tpAmb, caminho, exibeNaTela, a3);
        dynamic respostaJson = JsonConvert.DeserializeObject(retorno);
        String statusEnvio = respostaJson.statusEnvio;
        String statusConsulta = respostaJson.statusConsulta;
        String statusDownload = respostaJson.statusDownload;
        String cStat = respostaJson.cStat;
        String chNFSe = respostaJson.chNFSe;
        String chDPS = respostaJson.chDPS;
        String motivo = respostaJson.motivo;
        String nsNRec = respostaJson.nsNRec;
        dynamic erros = respostaJson.erros;

        // Testa se houve sucesso na emissão
        if (statusEnvio == "200" || statusEnvio == "-6")
        {

            // Testa se houve sucesso na consulta
            if (statusConsulta == "200")
            {

                // Testa se a nota foi autorizada
                if (cStat == "100")
                {
                    MessageBox.Show(motivo);

                    // Testa se o download teve problemas
                    if (statusDownload != "200")
                    {
                        // Aqui você pode realizar um tratamento em caso de erro no download
                        MessageBox.Show("Erro no Download");
                    }
                }
                else
                {
                    MessageBox.Show(motivo);
                }
            }
            else
            {
                MessageBox.Show(motivo + "\n" + erros);
            }
        }
        else
        {
            MessageBox.Show(motivo + "\n" + erros);
        }  

-----

Qualquer dúvida contate nossa equipe pelo WhatsApp ou/e HelpDesk. Obrigado pela atenção!!
