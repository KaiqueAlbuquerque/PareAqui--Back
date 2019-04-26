# PareAqui

BackEnd do APP PareAqui

Aplicativo desenvolvido como Trabalho de conclusão de curso na universidade São Judas Tadeu no ano de 2018.

Participantes:
Kaique Albuquerque - BackEnd
Matheus Lima - FrontEnd
Marcos Paes - Documentação
Rafaela Coelho - FrontEnd

Professor Orientador:
Nelson Aguiar

Descrição:
O aplicativo tem como objetivo permitir a divulgação de vagas e o aluguel das mesmas. Os alugueis podem ocorrer em 3 modalidades, sendo elas: avulso, diaria e mensal. 

Abaixo apresenta-se uma breve descrição do funcionamento do APP.

  - Donos de garagens e estacionamentos que possuam vagas disponiveis para alugar, cadastram as mesmas no aplicativo, informando endereço, modalidades de locação, preço, se aceita sugestão de preço e detalhes da vaga (se é coberta, se o portão é automático) e envia fotos da vaga.
  - A vaga fica pendente de aprovação. Os dados são enviados para o PareAqui e será aprovada ou recusada por um integrante da equipe. 
  - Caso seja aprovada, o dono da mesma é informado (push notification, sms e email) e a vaga aparece disponível no aplicativo para ser alugada. Se for reprovada, o motivo será informado ao dono da vaga por (push notification, sms e email).
  - Donos de veículos cadastram-se no aplicativo e cadastram também o seu veículo. Podem também opcionalmente cadastrar um cartão de crédito.
  - Dono do veículo ao ir para alguma região, procura no aplicativo se existem vagas disponíveis para serem alugadas nela. Para isto, ele entra no APP e será apresentada uma tela com um mapa para ele, ao digitar o endereço, serão mostradas as vagas disponíveis na região com seus respectivos preços e ao clicar na vaga aparecem detalhes como por exemplo as avaliações do dono da vaga, as modalidades disponíveis, as datas e os horários disponíveis para locação.
  - O preço na locação irá depender do que foi cadastrado na vaga. 
    * Se o dono da mesma aceitar sugestão de preço, o preço que aparecerá ao dono do veículo passará por nosso algoritmo de cálculo de preço e ao final o valor mostrado será proporcional aos das vagas da região (já incluindo a margem de lucro do PareAqui). 
    * Se não aceitar a sugestão de preço, será o preço que ele informou mais a margem de lucro do PareAqui.
  - Dono do veículo solicita a locação e o dono da vaga recebe notificações (push notification, sms e email) informando que alguém tem interesse na vaga dele.
  - É oferecido um canal de comunicação (chat) entre o dono da vaga e o solicitante da locação para que troquem informações.
  - Dono da vaga verifica no APP a solicitação de locação tendo acesso as avaliações do dono do veículo. 
    * Se aprovar a locação, o dono do veículo é notificado por (push notification, sms e email).
    * Se reprovar a locação, o dono do veículo também é notificado (push notification, sms e email) informando o motivo da recusa.
  - Antes do início da locação, o dono da vaga e do veículo poderão desistir da mesma. Uma taxa é cobrada de quem realizou o cancelamento para ser transferida a outra parte e cobrir o prejuizo causado neste cancelamento. A pessoa também é notificada por (push notification, sms e email).
  - Ao término da locação, o valor da mesma e eventuais multas por atraso serão debitadas do cartão de crédito cadastrado e será dada a opção do dono da vaga avaliar o dono do veículo e vice e versa.
  - Após 7 dias do término da locação, a PareAqui transfere para o proprietário da vaga, o valor do alugel.
  - Toda transação com cartão de crédito é feita com o PagSeguro, tudo dentro do App, ou seja, o usuário não é redirecionado para fora do aplicativo em nenhum momento.
  - Donos de estabelecimentos/ criador de eventos e donos de vagas, também podem cadastrar eventos. 
      * Para o dono da vaga, isto é vantajoso pois se houver algum evento cadastrado na região de suas vagas, e ele aceita a sujestão de preço, o valor do aluguel da vaga dele e de todos que aceitam sugestão de preço irá aumentar, pois a região terá uma demanda maior de pessoas procurando vagas para estacionar. 
      * Para o dono do evento/ estabelecimento, isto é bom pois irá aumentar a oferta de vagas na região, tornando seu estabelecimento mais acessivel.
  - Quando um evento é cadastrado, ele precisa ser aprovado pela equipe do PareAqui. Após aprovado, todos os proprietários de vagas na região do evento são notificados por (push notification, sms e email) sobre o evento para que ele saiba que em determinado dia e horário ele terá uma ótima oportunidade de obter lucros.
  
Documentação completa:
https://drive.google.com/open?id=1tQM_El9rsy6dOl_qnngHzlLerJ35mgAa
