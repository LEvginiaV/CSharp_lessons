import { action } from "@storybook/addon-actions";
import { storiesOf } from "@storybook/react";
import * as React from "react";
import { ServiceCalendarRecordDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordDto";
import { CustomerStatusDto, RecordStatusDto } from "../../api/new/dto/ServiceCalendar/ServiceCalendarRecordInfoDto";
import { WorkerServiceCalendarDayDto } from "../../api/new/dto/ServiceCalendar/WorkerServiceCalendarDayDto";
import { WorkingCalendarDayInfoDto } from "../../api/new/dto/WorkingCalendar/WorkersScheduleDtos";
// import { ShopServiceCalendarDayDto } from "../../api/new/dto/ServiceCalendar/ShopServiceCalendarDayDto";
import { fakeCustomers, fakeWorkers } from "../../api/new/FakeApis/fakeData";
import { fakeNomenclature } from "../../shellForTests/fakeNomenclature";
import { CalendarView } from "./Calendar";
import { CalendarRecord } from "./CalendarRecord";
import { StatusButtons } from "./StatusButtons";
import { WorkerColumnView } from "./WorkerColumn";

const record: ServiceCalendarRecordDto = {
  id: "1",
  recordStatus: RecordStatusDto.Active,
  customerStatus: CustomerStatusDto.Active,

  customerId: "1111",
  comment: "Какой-то длиннющий комментарий непонятно, о чем. Клиент любит летать в облаках",
  productIds: [fakeNomenclature[2].id, fakeNomenclature[5].id, fakeNomenclature[8].id],
  period: {
    startTime: "11:00:00",
    endTime: "13:00:00",
  },
};

const exampleWorkingTime: WorkingCalendarDayInfoDto = {
  date: new Date().toISOString(),
  records: [
    {
      period: {
        startTime: "08:00:00",
        endTime: "17:00:00",
      },
    },
  ],
};

const exampleWorkerCalendarDay: WorkerServiceCalendarDayDto = {
  workerId: "1111",
  date: new Date().toISOString(),
  records: [
    {
      ...record,
    },
  ],
};

const recordCommon = {
  nomenclature: fakeNomenclature,
  customer: fakeCustomers[0],
  onEdit: action("onEdit"),
  onChangeStatus: action("onChangeStatus"),
  onCancel: action("onCancel"),
  record,
};

const recordWrapStyles = {
  width: 280,
  paddingTop: 10,
  paddingLeft: 10,
};

storiesOf("Chart", module)
  .add("With full name", () => {
    return <CalendarView workers={fakeWorkers} />;
  })
  .add("WorkerColumn", () => {
    return (
      <WorkerColumnView
        nomenclature={fakeNomenclature}
        customers={fakeCustomers}
        info={exampleWorkerCalendarDay}
        workingTime={exampleWorkingTime.records}
        onStartAdd={action("StartAdd")}
        onEdit={action("onEdit")}
        onCancelRecord={action("onCancelRecord")}
        onChangeStatus={action("onChangeStatus")}
      />
    );
  })
  .add("StatusButtons", () => (
    <div style={recordWrapStyles}>
      <StatusButtons value={CustomerStatusDto.Active} onChange={action("onChange")} />
      <StatusButtons value={CustomerStatusDto.ActiveAccepted} onChange={action("onChange")} />
      <StatusButtons value={CustomerStatusDto.Completed} onChange={action("onChange")} />
    </div>
  ))
  .add("Record statuses", () => (
    <div style={recordWrapStyles}>
      <CalendarRecord {...recordCommon} />
      <CalendarRecord {...recordCommon} record={{ ...record, customerStatus: CustomerStatusDto.ActiveAccepted }} />
      <CalendarRecord {...recordCommon} record={{ ...record, customerStatus: CustomerStatusDto.Completed }} />
    </div>
  ))
  .add("Record sizes", () => (
    <div style={recordWrapStyles}>
      <CalendarRecord {...recordCommon} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "12:00:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:35:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:28:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:26:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:24:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:22:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:20:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:18:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:16:00" } }} />
      <CalendarRecord {...recordCommon} record={{ ...record, period: { ...record.period, endTime: "11:14:00" } }} />
    </div>
  ))
  .add("Record empty fields", () => (
    <div style={recordWrapStyles}>
      <CalendarRecord {...recordCommon} customer={null} />
      <CalendarRecord {...recordCommon} customer={{ ...fakeCustomers[0], name: "" }} />
      <CalendarRecord {...recordCommon} customer={{ ...fakeCustomers[0], phone: "" }} />
      <CalendarRecord {...recordCommon} customer={{ ...fakeCustomers[0], comment: "" }} />
    </div>
  ))
  .add("Record productIds", () => (
    <div style={recordWrapStyles}>
      <CalendarRecord {...recordCommon} record={{ ...record, productIds: [] }} />
      <CalendarRecord {...recordCommon} record={{ ...record, productIds: [fakeNomenclature[0].id] }} />
      <CalendarRecord
        {...recordCommon}
        record={{ ...record, productIds: [fakeNomenclature[0].id, fakeNomenclature[1].id] }}
      />
      <CalendarRecord
        {...recordCommon}
        record={{ ...record, productIds: [fakeNomenclature[0].id, fakeNomenclature[1].id, fakeNomenclature[2].id] }}
      />
    </div>
  ));
