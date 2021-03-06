import { Component, OnInit } from '@angular/core';
import { EventModel } from '../../shared/models/event.model';
import { EventListModel } from '../../shared/models/eventList.model';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { EventService } from '../../shared/services/event.service';

@Component({
  selector: 'app-event-details',
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.css']
})
export class EventDetailsComponent implements OnInit {
  joinnable = false;
  id: string;
  event: EventModel = new EventModel(
    null,
    null,
    null,
    null,
    null,
    null,
    null,
    null
  );

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private router: Router
  ) {
    this.route.params.subscribe((params: Params) => {
      if (params['id'] === undefined) {
        this.id = '';
      } else {
        this.id = '' + params['id'];
      }
    });
    this.eventService
      .joinnable(this.id)
      .subscribe(
        response => (this.joinnable = false),
        error => (this.joinnable = true)
      );
  }
  ngOnInit(): void {
    this.eventService
      .getEvent(this.id)
      .subscribe(event => (this.event = event), error => console.log(error));
  }
}
