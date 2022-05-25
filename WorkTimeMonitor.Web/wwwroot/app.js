(function () {
  const itemClicked = function (sender) {
    const {
      target: {
        dataset: { cardid },
      },
    } = sender;

    $.get(`api/cards/${cardid}/history`, function (res) {
      const { cardId, userName, history } = res;
      let historyRows = "";

      history.forEach((element) => {
        const { status, timeStamp } = element;
        historyRows += `<tr>
            <td>${status}</td>
            <td>${timeStamp}</td>
          </tr>`;
      });

      let historyHtml = `<hr /><h2>Card id: ${cardId}, user: ${userName}</h2>
        <table class="table"><thead><tr><th>Status/th><th>Timestamp</th></tr></thead>
        <tbody>${historyRows}</tbody>
        </table>
      `;

      $("#cards-history").html(historyHtml);
    });
  };

  $.get("api/cards", function (res) {
    let table =
      "<table class='table'><thead><tr><th>User name</th><th>Card id</th></tr></thead><tbody>";

    res.forEach((element) => {
      const { userName, cardId } = element;
      table += `<tr>
        <td>${userName}</td>
        <td class="card-item" data-cardid="${cardId}">${cardId}</td>
      </tr>`;
    });
    table += "</tbody></table>";

    $("#cards").html(table);
    $(".card-item").click(itemClicked);
  });
})();
